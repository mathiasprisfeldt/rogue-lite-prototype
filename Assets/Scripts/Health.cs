using System;
using System.Collections;
using AcrylecSkeleton.Utilities;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Health
{
    enum HealthType
    {
        Normal,
        Container
    }

    /// <summary>
    /// Purpose: Controls life of an object.
    /// Creator: Mathias Prisfeldt
    /// </summary>
    [DisallowMultipleComponent]
    public class Health : MonoBehaviour 
    {
        private bool _isLateChecking; //Are we checking in end of frame if we're dead?

        #region Inspector Fields

        [SerializeField]
        private HealthType _healthType;

        [SerializeField, Tooltip("If true, waits to see if you died until end of frame.")]
        private bool _lateCheck; //Should you be able to take damage down to 0 and get healed and saved?

        [SerializeField]
        private bool _isInvurnable;

        [SerializeField]
        private bool _invurnableTakeDmg; //Do I still take dmg even if im invurnable?

        [SerializeField]
        private float _healthAmount = 100; //Amount of health

        [SerializeField]
        private Vector2 _healthInterval = new Vector2(0, 100); //Health is clamped to these values.

        [SerializeField, Tooltip("If its empty it uses GameObject.")]
        private GameObject _responsibleGameObject; //The gameobject responsible for health (The object that gets destroyed when dead)

        [Space, SerializeField]
        private bool _destroyOnDead = true;

        [SerializeField]
        private UnityEvent _deadEvent;

        #endregion

        public bool IsDead { get; set; }

        public UnityEvent DeadEvent
        {
            get { return _deadEvent; }
            set { _deadEvent = value; }
        } //Event invoked when dead

        public bool IsInvurnable
        {
            get { return _isInvurnable; }
            set
            {
                _isInvurnable = value; 
                CheckHealth();
            }
        }

        public float HealthAmount
        {
            get { return _healthAmount; }
            set
            {
                float newHealth = Mathf.Clamp(value, _healthInterval.x, _healthInterval.y);

                if (IsInvurnable)
                {
                    if (_invurnableTakeDmg)
                        _healthAmount = newHealth;

                    return;
                }

                _healthAmount = newHealth;

                if (_lateCheck)
                {
                    if (!_isLateChecking)
                    {
                        StartCoroutine(LateCheckHealth());
                        _isLateChecking = true;
                    }
                }
                else
                    CheckHealth();
            }
        }

        /// <summary>
        /// Deals amount of damage to object, if it exceeds 0 its dead.
        /// NOTE: If container its calculated in container sizes.
        /// Ex. dmg = 1 = 1 heart. dmg = 0.5f = 0.5 heart.
        /// It always rounds to halfs or wholes.
        /// </summary>
        /// <param name="dmg">Amount of damage to deal.</param>
        public void Damage(float dmg)
        {
            var amountToDmg = dmg;

            switch (_healthType)
            {
                case HealthType.Container:
                    MathUtils.RoundToNearest(dmg, 2);
                    amountToDmg = dmg * GSManager.Instance.HealthContainerSize;
                    break;
            }

            HealthAmount -= amountToDmg;
        }

        /// <summary>
        /// Heals the object with chosen amount.
        /// </summary>
        /// <param name="health">Amount to heal</param>
        public void Heal(float health)
        {
            HealthAmount += health;
        }

        /// <summary>
        /// Checks if the played died this game loop.
        /// </summary>
        public void CheckHealth()
        {
            bool gotKilled = HealthAmount <= 0;

            if (!IsDead && gotKilled)
            {
                DeadEvent.Invoke();

                if (_destroyOnDead)
                    Destroy(_responsibleGameObject ? _responsibleGameObject : gameObject);
            }

            IsDead = gotKilled;
        }

        /// <summary>
        /// Returns size of size in heal container sizes.
        /// </summary>
        public float GetContainerSize()
        {
            return _healthAmount / GSManager.Instance.HealthContainerSize;
        }

        /// <summary>
        /// Used for checking if the player died last game loop.
        /// </summary>
        /// <returns></returns>
        IEnumerator LateCheckHealth()
        {
            yield return new WaitForEndOfFrame();
            CheckHealth();
            _isLateChecking = false;
        }
    }
}