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
            Projectile projectile = Instantiate(_projectile, transform.position.ToVector2() + _spawnPos, Quaternion.identity);
            projectile.Direction = new Vector2(Context.M.Character.LookDirection, 0);
            projectile.Shoot();
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