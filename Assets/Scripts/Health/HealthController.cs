using System;
using System.Collections;
using AcrylecSkeleton.Extensions;
using AcrylecSkeleton.Utilities;
using Controllers;
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
    [RequireComponent(typeof(HealthController))]
    public class HealthController : MonoBehaviour
    {
        private bool _isLateChecking; //Are we checking in end of frame if we're dead?

        #region Inspector Fields

        [SerializeField]
        private HealthType _healthType;

        [SerializeField, Tooltip("If true, waits to see if you died until end of frame.")]
        private bool _lateCheck; //Should you be able to take damage down to 0 and get healed and saved?

        [SerializeField]
        private bool _invurnableOnDmg; //Do you get invurnability when you take damage?

        [SerializeField]
        private float _invurnabilityDuration; //When you take damge how long will it last?

        [SerializeField]
        private bool _isInvurnable;

        [SerializeField]
        private bool _dmgWhileInvurnable; //Do I still take dmg even if im invurnable?

        [SerializeField]
        private float _healthAmount = 100; //Amount of health

        [SerializeField]
        private Vector2 _healthInterval = new Vector2(0, 100); //Health is clamped to these values.

        [SerializeField, Tooltip("If its empty it uses GameObject.")]
        private GameObject _responsibleGameObject; //The gameobject responsible for health (The object that gets destroyed when dead)

        [Space, SerializeField, Tooltip("If true, it wont kill itself, you must do it yourself.")]
        private bool _destroyOnDead = true;

        [SerializeField]
        private UnityEvent _deadEvent;

        [Header("References:"), Space]
        [SerializeField]
        private Character _character;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

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
                    if (_dmgWhileInvurnable)
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

        public Character Character
        {
            get { return _character; }
            set { _character = value; }
        }

        /// <summary>
        /// Deals amount of damage to object, if it exceeds 0 its dead.
        /// NOTE: If container its calculated in container sizes.
        /// Ex. dmg = 1 = 1 heart. dmg = 0.5f = 0.5 heart.
        /// It always rounds to halfs or wholes.
        /// </summary>
        /// <param name="dmg">Amount of damage to deal.</param>
        public void Damage(float dmg, bool giveInvurnability = false, Transform from = null)
        {
            if (dmg <= 0 || IsDead)
                return;

            var amountToDmg = dmg;

            switch (_healthType)
            {
                case HealthType.Container:
                    MathUtils.RoundToNearest(dmg, 2);
                    amountToDmg = dmg * GSManager.Instance.HealthContainerSize;
                    break;
            }

            //Apply knockback
            if (from)
                _character.KnockbackHandler.AddForce(from.position.ToVector2().DirectionTo(_character.Rigidbody.position) * 3, .1f);

            HealthAmount -= amountToDmg;

            if (giveInvurnability || _invurnableOnDmg)
                StartCoroutine(StartInvurnability(_invurnabilityDuration));
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
            bool gotKilled = HealthAmount <= _healthInterval.x;

            if (!IsDead && gotKilled)
            {
                DeadEvent.Invoke();

                if (_destroyOnDead)
                    Kill();
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
        /// Used to kill an object the right way.
        /// </summary>
        public void Kill()
        {
            IsDead = true;
            HealthAmount = _healthInterval.x;
            Destroy(_responsibleGameObject ? _responsibleGameObject : gameObject);
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

        /// <summary>
        /// Starts a invurnablity timer.
        /// </summary>
        /// <param name="duration">Amount of time you're invurnable</param>
        /// <returns></returns>
        IEnumerator StartInvurnability(float duration)
        {
            if (!IsDead && !IsInvurnable) //If your not dead and not invurnable start the invurnablity timer
            {
                //If we have a spriterender, save its color.
                Color savedColor = Color.white;
                if (_spriteRenderer)
                {
                    savedColor = _spriteRenderer.color;
                    _spriteRenderer.color = Color.red;
                }

                float timer = duration;
                IsInvurnable = true;

                while (timer > 0 && IsInvurnable && !IsDead)
                {
                    yield return new WaitForEndOfFrame();
                    timer -= Time.unscaledDeltaTime / duration;
                }

                IsInvurnable = false;

                //Set spriterenderer back to tis original color.
                if (_spriteRenderer)
                    _spriteRenderer.color = savedColor;
            }
        }
    }
}