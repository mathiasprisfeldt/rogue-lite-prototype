using AcrylecSkeleton.Extensions;
using Projectiles;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose: Basic class for all enemies that can shoot.
    /// Creator: MP
    /// </summary>
    public class EnemyShoot : EnemyAttack
    {
        [SerializeField, Tooltip("Recipe for projectile.")]
        private Projectile _projectile;

        [SerializeField, Tooltip("Spawn position of the projectile, relative to itself.")]
        private Vector2 _spawnPos;

        public override void Attack()
        {
            int dir = Context.M.Character.LookDirection;
            Vector2 spawnPos = new Vector2(_spawnPos.x, 0) * dir + new Vector2(0, _spawnPos.y);

            Projectile projectile = Instantiate(_projectile, transform.position.ToVector2() + spawnPos, Quaternion.identity);
            projectile.Direction = new Vector2(dir, 0);
            projectile.Shoot();
             
            base.Attack();
        }

        protected override void OnDrawGizmosSelected()
        {
            if (!_drawGizmos || !enabled)
                return;

            Gizmos.DrawSphere(_spawnPos + transform.position.ToVector2(), 0.15f);

            base.OnDrawGizmosSelected();
        }

        
    }
}