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
    public abstract class EnemyAttack : EnemyState
    {
        private bool _isAttacking;
        private float _indicatorTimer;
        private float _cooldownTimer;
        private bool _canAttack;
        private Collider2D[] _hitboxResults = new Collider2D[10];

        [SerializeField]
        protected bool _drawGizmos;

        [SerializeField]
        private Vector2 _attackBoxSize = Vector2.one;

        [SerializeField]
        private Vector2 _attackBoxOffset = Vector2.right;

        public Vector2 Hitbox
        {
            get
            {
                bool isLeft = false;

                if (Context && Context.M.Character)
                    isLeft = Context.M.Character.LookDirection == -1;

                Vector2 relPos = new Vector2(isLeft ? -_attackBoxOffset.x : _attackBoxOffset.x, 0);
                return new Vector2(0, _attackBoxOffset.y) + relPos + transform.position.ToVector2();
            }
        }

        void Start()
        {
            _indicatorTimer = Context.M.IndicatorDuration;
            _cooldownTimer = Context.M.AttackCooldown;
        }

        void Update()
        {
            if (_isAttacking)
                return;

            //Make sure theres a cooldown on attack.
            if (!_canAttack)
            {
                _cooldownTimer -= Time.deltaTime;

                if (_cooldownTimer <= 0)
                {
                    _canAttack = true;
                }
            }
        }

        public override void Begin()
        {
            Context.M.Character.StandStill();
            base.Begin();
        }

        public override void End()
        {
            base.End();
            _isAttacking = false;
        }

        public override void Think(float deltaTime)
        {
            if (_isAttacking)
                return;

            //Attack when player gets close.
            if (_canAttack)
            {
                _indicatorTimer -= deltaTime;

                if (_indicatorTimer <= 0)
                {
                    PreAttack();
                }
            }
        }

        public override void Reason()
        {
            if (_isAttacking)
                return;

            if (!CheckHitbox() && !_canAttack || !Context.M.Target)
                ChangeState<EnemyIdle>();

            base.Reason();
        }

        public override bool ShouldTakeover()
        {
            if (_isAttacking)
                return true;

            if (CheckHitbox() && Context.M.Target && _canAttack && !Context.C.IsTurning)
                return true;

            return false;
        }

        /// <summary>
        /// Base attack method for any attack related logic.
        /// </summary>
        public virtual void Attack()
        {
            _cooldownTimer = Context.M.AttackCooldown;
            _indicatorTimer = Context.M.IndicatorDuration;
            _isAttacking = false;
            _canAttack = false;
            SetCombat(false);
        }

        /// <summary>
        /// Gets called before attack logic happens.
        /// Usually handles which one calls the attack method.
        /// </summary>
        private void PreAttack()
        {
            _isAttacking = true;

            //Play attack animation
            if (Context.M.Character.MainAnimator && Context.M.AttackAnim)
            {
                Context.M.Character.MainAnimator.SetTrigger("Attack");
                SetCombat(true);
            }
                
            else
                Attack();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (!_drawGizmos || !enabled)
                return;

            if (enabled && Context && _indicatorTimer != Context.M.IndicatorDuration)
                Gizmos.DrawSphere(transform.position.ToVector2() + new Vector2(0, 1), .15f);

            Gizmos.DrawWireCube(Hitbox, _attackBoxSize);
        }

        /// <summary>
        /// Is the player hitting our hitbox?
        /// </summary>
        /// <returns>True if it hit something good.</returns>
        public bool CheckHitbox()
        {
            //TODO: Properly needs optimizing
            int hitCount = Physics2D.OverlapBoxNonAlloc(Hitbox, _attackBoxSize, 0, _hitboxResults, LayerMask.GetMask("Hitbox"));

            for (int i = 0; i < hitCount; i++)
            {
                if (_hitboxResults[i].CompareTag("Player"))
                    return true;
            }

            return false;
        }

        public void SetCombat(bool boolean)
        {
            Context.M.Character.MainAnimator.SetBool("InCombat", boolean);
        }
    }
}