using AcrylecSkeleton.Extensions;
using Archon.SwissArmyLib.Automata;
using Enemy;
using Projectiles;
using UnityEngine;

namespace EnemyShoot
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
            Instantiate(_projectile, transform.position.ToVector2() + _spawnPos, Quaternion.identity);

            base.Attack();
        }

        protected override void OnDrawGizmosSelected()
        {
            if (!_drawGizmos)
                return;

            Gizmos.DrawSphere(_spawnPos + transform.position.ToVector2(), 0.15f);

            base.OnDrawGizmosSelected();
        }
    }
}