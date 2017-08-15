using System.Linq;
using AcrylecSkeleton.Extensions;
using Assets.Enemy;
using Controllers;
using Managers;
using UnityEngine;

namespace EnemyAttack
{
    /// <summary>
    /// Purpose: Base class for all combat behaviours.
    /// Creator: MP
    /// </summary>
    public class EnemyAttack : EnemyState
    {
        [SerializeField]
        private bool _drawGizmos;

        [SerializeField]
        private Vector2 _attackBoxSize;

        [SerializeField]
        private Vector2 _attackBoxOffset;

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
                isLeft = App.M.Character.LookDirection == LookDirection.Left;

            Vector2 relPos = isLeft ? -_attackBoxOffset : _attackBoxOffset;
            return relPos + transform.position.ToVector2();
        }

        void Start()
        {
            _indicatorTimer = App.M.IndicatorDuration;
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

            if (!IsActive)
                return;

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

            //HACKED: Should properly change when AI becomes more dynamic
            if (!App.M.Target && !_canAttack)
                App.C.ChangeState<EnemyPatrol>();
        }

        /// <summary>
        /// Base attack method for any attack related logic.
        /// </summary>
        public virtual void Attack()
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(GetHitbox(), _attackBoxSize, 0, LayerMask.GetMask("Player"));

            if (hits.Any())
                GameManager.Instance.Player.M.ActionController.HealthController.Damage(App.M.Character.Damage);

            _canAttack = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos)
                return;

            if (_indicatorTimer != App.M.IndicatorDuration)
                Gizmos.DrawSphere(transform.position.ToVector2() + new Vector2(0, 1), .15f);

            Gizmos.DrawWireCube(GetHitbox(), _attackBoxSize);
        }

        public override bool CheckPrerequisite()
        {
            if (!IsActive)
            {
                Collider2D[] hits = Physics2D.OverlapBoxAll(GetHitbox(), _attackBoxSize, 0, LayerMask.GetMask("Player"));
                return hits.Any();
            }

            return false;
        }
    }
}