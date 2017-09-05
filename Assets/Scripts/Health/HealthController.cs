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
        Container,
        Normal
    }

    [Serializable]
    public class OnDamageEvent : UnityEvent<Character>
    {
        
    }

    /// <summary>
    /// Purpose: Controls life of an object.
    /// Creator: Mathias Prisfeldt
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HealthController))]
    public class HealthController : MonoBehaviour
    {
        private bool _isDead;
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

        [SerializeField]
        private float _knockbackForce = 300;

        [SerializeField]
        private float _knockbackDuration = 0.2f;

        [SerializeField, Tooltip("If its empty it uses GameObject.")]
        private GameObject _responsibleGameObject; //The gameobject responsible for health (The object that gets destroyed when dead)

        [Space, SerializeField, Tooltip("If true, it wont kill itself, you must do it yourself.")]
        private bool _destroyOnDead = true;

        [SerializeField]
        private bool _trapImmune;

        [SerializeField]
        private UnityEvent _onDead;

        [SerializeField]
        private OnDamageEvent _onDamage;


        [Header("References:"), Space]
        [SerializeField]
        private Character _character;

        #endregion

        public UnityEvent OnHealEvent;

        public bool IsDead
        {
            get { return _isDead; }
            set
            {
                //Updating character animator with dead data.
                if (Character.MainAnimator && _isDead != value)
                {
                    Character.MainAnimator.SetBool("Dead", value);
                    Character.MainAnimator.SetTrigger("Die");
                }

                _isDead = value; 
            }
        }

        //Event invoked when dead
        public UnityEvent OnDead
        {
            get { return _onDead; }
            set { _onDead = value; }
        }

        public OnDamageEvent OnDamage
        {
            get { return _onDamage; }
            set { _onDamage = value; }
        }

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

        public bool TrapImmune
        {
            get { return _trapImmune; }
        }

        void Awake()
        {
            OnDamage = new OnDamageEvent();
        }

        /// <summary>
        /// Deals amount of damage to object, if it exceeds 0 its dead.
        /// NOTE: If container its calculated in container sizes.
        /// Ex. dmg = 1 = 1 heart. dmg = 0.5f = 0.5 heart.
        /// It always rounds to halfs or wholes.
        /// </summary>
        /// <param name="dmg">Amount of damage to deal.</param>
        /// <param name="pos">Position from where the damage came from</param>
        /// <param name="from">Did the damage come from a specific character?</param>
        public void Damage(float dmg, bool giveInvurnability = true, Vector2 pos = default(Vector2), Character from = null)
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

            HealthAmount -= amountToDmg;

            //Apply knockback
            if (pos != Vector2.zero && !_isInvurnable && HealthAmount > 0)
            {
                var dir = pos.DirectionTo(_character.Rigidbody.position);
                if (Math.Abs(dir.y) < 0.01f)
                    dir.y = 1f;
                _character.KnockbackHandler.AddForce( dir * _knockbackForce, _knockbackDuration);
            }
                

            //If we take damage show hit animation.
            //But dont show if we're invurnable (Only if we take dmg while being it)
            if (Character.MainAnimator)
            {
                if (_isInvurnable)
                {
                    if (_dmgWhileInvurnable)
                        Character.MainAnimator.SetTrigger("Hit");
                }
                else
                    Character.MainAnimator.SetTrigger("Hit");
            }

            if (!IsInvurnable)
                OnDamage.Invoke(from);

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
            OnHealEvent.Invoke();
        }

        /// <summary>
        /// Checks if the played died this game loop.
        /// </summary>
        public void CheckHealth()
        {
            bool gotKilled = HealthAmount <= _healthInterval.x;

            if (!IsDead && gotKilled)
            {
                IsDead = true;
                OnDead.Invoke();

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
                float timer = duration;
                IsInvurnable = true;

                while (timer > 0 && IsInvurnable && !IsDead)
                {
                    yield return new WaitForEndOfFrame();
                    timer -= Time.unscaledDeltaTime / duration;
                }

                IsInvurnable = false;
            }
        }

        void OnDestroy()
        {
            OnDamage.RemoveAllListeners();
            OnDead.RemoveAllListeners();
            OnHealEvent.RemoveAllListeners();
        }
    }
}