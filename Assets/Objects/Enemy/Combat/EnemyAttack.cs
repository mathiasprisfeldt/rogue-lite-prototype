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

            if (App && App.M.Character)
                isLeft = App.M.Character.LookDirection == -1;

            Vector2 relPos = isLeft ? -_attackBoxOffset : _attackBoxOffset;
            return relPos + transform.position.ToVector2();
        }

        void Start()
        {
            _indicatorTimer = App.M.IndicatorDuration;
            _cooldownTimer = App.M.AttackCooldown;
        }

        void Update()
        {
            //Make sure theres a cooldown on attack.
            if (!_canAttack)
            {
                _cooldownTimer -= Time.deltaTime;

                if (_cooldownTimer <= 0)
                {
                    _cooldownTimer = App.M.AttackCooldown;
                    _canAttack = true;
                }
            }
        }

        public override void StateUpdate()
        {
            //Attack when player gets close.
            if (_canAttack)
            {
                _indicatorTimer -= Time.deltaTime;

                if (_indicatorTimer <= 0)
                {
                    _indicatorTimer = App.M.IndicatorDuration;
                    Attack();
                }
            }

            //TODO: HACKED: Should properly change when AI becomes more dynamic
            if (!CheckHitbox() && !_canAttack && IsIsolated)
                App.C.ResetToLast();
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

            if (_indicatorTimer != App.M.IndicatorDuration)
                Gizmos.DrawSphere(transform.position.ToVector2() + new Vector2(0, 1), .15f);

            Gizmos.DrawWireCube(GetHitbox(), _attackBoxSize);
        }

        public override bool CheckPrerequisite()
        {
            return !IsActive && CheckHitbox() && App.M.Target && !App.C.IsState<EnemyAvoid>();
        }

        /// <summary>
        /// Is the player hitting our hitbox?
        /// </summary>
        /// <returns>True if it hit something good.</returns>
        public bool CheckHitbox()
        {
            //TODO: Properly needs optimizing
            return Physics2D.OverlapBoxAll(GetHitbox(), _attackBoxSize, 0, LayerMask.GetMask("Hitbox")).Any(d => d.tag == "Player");
        }
    }
}