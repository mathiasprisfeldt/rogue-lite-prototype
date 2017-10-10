using System;
using System.Collections;
using AcrylecSkeleton.Extensions;
using AcrylecSkeleton.Utilities;
using Archon.SwissArmyLib.ResourceSystem;
using Archon.SwissArmyLib.Utils;
using Controllers;
using ItemSystem;
using Managers;
using Spriter2UnityDX;
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
        //Flash indication fields
        private Color _originalColor;
        private float _flashTimer;

        private bool _isDead;
        private bool _isLateChecking; //Are we checking in end of frame if we're dead?

        #region Inspector Fields

        #region Damage indication

        [Header("Flash indication:")]
        [SerializeField]
        private AnimationCurve _flashCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        [SerializeField]
        private Color _flashColor = new Color(255, 39, 39, 255);

        [SerializeField, Tooltip("In seconds, if 0 it doesn't flash.")]
        private float _flashDuration = .15f;

        #endregion

        [Header("Settings:"), Space]
        [SerializeField]
        private GameObject _hitEffectPrefab;

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
        private float _maxHealth = 3;

        [SerializeField]
        private Vector2 _healthInterval = new Vector2(0, 15); //Health is clamped to these values.

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

        [SerializeField]
        private Collider2D _hitBox;

        [SerializeField, Tooltip("Used for flash indication.")]
        private EntityRenderer _entityRenderer;

        [SerializeField, Tooltip("Used for flash indication.")]
        private SpriteRenderer _spriteRenderer;

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

        public bool HitboxEnabled
        {
            get { return _hitBox.gameObject != null && _hitBox.gameObject.activeInHierarchy; }
            set
            {
                if (_hitBox.gameObject != null)
                    _hitBox.gameObject.SetActive(value);
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
                float newHealth = Mathf.Clamp(value, HealthInterval.x, HealthInterval.y);

                if (IsInvurnable && _healthAmount > newHealth)
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

        public float LastDamageRcieved { get; set; }

        public bool TrapImmune
        {
            get { return _trapImmune; }
        }

        public Vector2 HealthInterval
        {
            get { return _healthInterval; }
            set { _healthInterval = value; }
        }

        public float MaxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = Mathf.Clamp(value,_healthInterval.x,_healthInterval.y); }
        }

        void Awake()
        {
            OnDamage = new OnDamageEvent();
        }

        void Start()
        {
            if (_spriteRenderer)
                _originalColor = _spriteRenderer.color;

            if (_entityRenderer)
                _originalColor = _entityRenderer.Color;

            CheckHealth();
        }

        void Update()
        {
            if (_flashTimer > 0)
            {
                _flashTimer -= BetterTime.DeltaTime / _flashDuration;

                var targetColor = _flashTimer <= 0
                    ? _originalColor
                    : Color.Lerp(_originalColor, _flashColor, _flashCurve.Evaluate(_flashTimer));

                if (_entityRenderer)
                    _entityRenderer.Color = targetColor;

                if (_spriteRenderer)
                    _spriteRenderer.color = targetColor;
            }
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
        /// <param name="ignoreInvurnability">Should we force damage onto health controller?</param>
        /// <param name="triggerItemhandler">Used to make direct damage that shouldn't call item handlers' <see cref="ItemHandler.OnHit"/></param>
        public void Damage(float dmg, bool giveInvurnability = false, Vector2 pos = default(Vector2), Character from = null, bool ignoreInvurnability = false, bool triggerItemhandler = true)
        {
            if (dmg <= 0 || IsDead)
                return;

            bool giveDamage = (!IsInvurnable || _dmgWhileInvurnable) || ignoreInvurnability;

            var amountToDmg = dmg;

            switch (_healthType)
            {
                case HealthType.Container:
                    MathUtils.RoundToNearest(dmg, 2);
                    amountToDmg = dmg * GSManager.Instance.HealthContainerSize;
                    break;
            }

            if (giveDamage)
            {
                //If damage dealer has a Hit items, find them and give them a call.
                if (from != null && from.ItemHandler && triggerItemhandler)
                {
                    LastDamageRcieved = amountToDmg;
                    from.ItemHandler.OnHit(this);
                }
            }

            HealthAmount -= amountToDmg;

            //Create hit effect
            if (_hitEffectPrefab && (from || pos != Vector2.zero) && giveDamage)
            {
                Bounds hitBounds = _hitBox.bounds;
                hitBounds.Expand(-.5f);

                Vector2 point = hitBounds.ClosestPoint(from ? from.Origin : pos);
                Instantiate(_hitEffectPrefab, point, Quaternion.identity);
            }

            //Flash indication
            if (!_flashDuration.FastApproximately(0) && giveDamage)
                _flashTimer = 1;

            if (IsDead)
                return;

            //Apply knockback
            if (pos != Vector2.zero && giveDamage)
            {
                var dir = pos.DirectionTo(_character.Rigidbody.position);
                if (Math.Abs(dir.y) < 0.01f)
                    dir.y = 1f;
                _character.KnockbackHandler.AddForce(dir * _knockbackForce, _knockbackDuration);
            }

            //If we take damage show hit animation.
            //But dont show if we're invurnable (Only if we take dmg while being it)
            if (Character.MainAnimator && giveDamage)
            {
                if (_dmgWhileInvurnable)
                    Character.MainAnimator.SetTrigger("Hit");
            }

            if (giveInvurnability || _invurnableOnDmg)
                StartCoroutine(StartInvurnability(_invurnabilityDuration));

            if (giveDamage)
                OnDamage.Invoke(from);
        }

        /// <summary>
        /// Heals the object with chosen amount.
        /// </summary>
        /// <param name="health">Amount to heal</param>
        public void Heal(float health)
        {
            var oldHealth = HealthAmount;
            HealthAmount = Mathf.Clamp(HealthAmount + health,_healthInterval.x,MaxHealth);
            if(HealthAmount != oldHealth)
                OnHealEvent.Invoke();
        }

        /// <summary>
        /// Checks if the played died this game loop.
        /// </summary>
        public void CheckHealth()
        {
            bool gotKilled = HealthAmount <= HealthInterval.x;

            if (!IsDead && gotKilled)
            {
                IsDead = true;

                if (_destroyOnDead)
                        Kill();
                OnDead.Invoke();

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
            HealthAmount = HealthInterval.x;
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
                    timer -= BetterTime.UnscaledDeltaTime / duration;
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

        /// <summary>
        /// Checks to see if specific amount of damage would kill the health controller.
        /// </summary>
        public bool WouldKill(float damage)
        {
            return HealthAmount - damage <= HealthInterval.x;
        }
    }
}