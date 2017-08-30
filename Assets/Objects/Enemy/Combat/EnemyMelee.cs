using AcrylecSkeleton.Extensions;
using Enemy;
using Managers;
using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Enemy
{
    /// <summary>
    /// Purpose: Base class for any melee attack states for enemies.
    /// Creator: MP
    /// </summary>
    public class EnemyMelee : EnemyAttack 
    {
        [Header("Attack Hitbox:"), SerializeField]
        private Vector2 _attackBoxSize = Vector2.one;

        [SerializeField]
        private Vector2 _attackBoxOffset = Vector2.right;

        public override void Attack()
        {
            if (IsActive && CheckHitbox(_attackBoxOffset, _attackBoxSize))
                GameManager.Instance.Player.M.ActionController.HealthController.Damage(Context.M.Character.Damage, from: Context.M.Character, pos: transform.position);

            base.Attack();
        }

        protected override void OnDrawGizmosSelected()
        {
            if (!_drawGizmos || !enabled)
                return;

            Gizmos.DrawWireCube(GetHitboxPos(_attackBoxOffset), _attackBoxSize);

            base.OnDrawGizmosSelected();
        }
    }
}