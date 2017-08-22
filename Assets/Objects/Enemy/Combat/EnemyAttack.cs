using System.Linq;
using AcrylecSkeleton.Extensions;
using Controllers;
using Managers;
using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Enemy
{
    /// <summary>
    /// Purpose: Base class for all combat behaviours.
    /// Creator: MP
    /// </summary>
    public class EnemyAttack : EnemyState
    {
        private Collider2D[] _hitboxResults = new Collider2D[10];

        [SerializeField]
        protected bool _drawGizmos;

        [SerializeField]
        private Vector2 _attackBoxSize = Vector2.one;

        [SerializeField]
        private Vector2 _attackBoxOffset = Vector2.right;

        private float _indicatorTimer;
        private float _cooldownTimer;
        private bool _canAttack = true;

        /// <summary>
        /// Returns attack hitbox taking settings into account.
        /// </summary>
        public Vector2 GetHitbox()
        {
            bool isLeft = false;

            if (Context && Context.M.Character)
                isLeft = Context.M.Character.LookDirection == -1;

            Vector2 relPos = isLeft ? -_attackBoxOffset : _attackBoxOffset;
            return relPos + transform.position.ToVector2();
        }

        void Start()
        {
            _indicatorTimer = Context.M.IndicatorDuration;
            _cooldownTimer = Context.M.AttackCooldown;
        }

        void Update()
        {
            //Make sure theres a cooldown on attack.
            if (!_canAttack)
            {
                _cooldownTimer -= Time.deltaTime;

                if (_cooldownTimer <= 0)
                {
                    _cooldownTimer = Context.M.AttackCooldown;
                    _canAttack = true;
                }
            }
        }

        public override void Think(float deltaTime)
        {
            //Attack when player gets close.
            if (_canAttack)
            {
                _indicatorTimer -= Time.deltaTime;

                if (_indicatorTimer <= 0)
                {
                    _indicatorTimer = Context.M.IndicatorDuration;
                    Attack();
                }
            }
        }

        public override void End()
        {
            base.End();


            _indicatorTimer = Context.M.IndicatorDuration;
            _cooldownTimer = Context.M.AttackCooldown;
        }

        public override void Reason()
        {
            if (!CheckHitbox() && !_canAttack)
                ChangeState<EnemyIdle>();

            base.Reason();
        }

        public override bool ShouldChange()
        {
            if (CheckHitbox() && Context.M.Target)
                return true;

            return false;
        }

        /// <summary>
        /// Base attack method for any attack related logic.
        /// </summary>
        public virtual void Attack()
        {
            _canAttack = false;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (!_drawGizmos)
                return;

            if (enabled && Context && _indicatorTimer != Context.M.IndicatorDuration)
                Gizmos.DrawSphere(transform.position.ToVector2() + new Vector2(0, 1), .15f);

            Gizmos.DrawWireCube(GetHitbox(), _attackBoxSize);
        }

        /// <summary>
        /// Is the player hitting our hitbox?
        /// </summary>
        /// <returns>True if it hit something good.</returns>
        public bool CheckHitbox()
        {
            //TODO: Properly needs optimizing
            int hitCount = Physics2D.OverlapBoxNonAlloc(GetHitbox(), _attackBoxSize, 0, _hitboxResults, LayerMask.GetMask("Hitbox"));

            for (int i = 0; i < hitCount; i++)
            {
                if (_hitboxResults[i].CompareTag("Player"))
                    return true;
            }

            return false;
        }
    }
}