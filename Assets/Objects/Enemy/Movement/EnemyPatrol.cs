using AcrylecSkeleton.StateKit;
using CharacterController;
using UnityEngine;

namespace Assets.Enemy
{
    /// <summary>
    /// Purpose: Patrol pattern for all enemies patrolling an area
    /// Creator: MP
    /// </summary>
    public class EnemyPatrol : EnemyState
    {
        private Vector2 _patrolDirection = Vector2.right;

        void FixedUpdate()
        {
            if (!IsActive)
                return;

            int bumpingDirection = App.M.Character.BumpingDirection;

            //If we're bumping into something, change direction.
            if (bumpingDirection != 0)
                _patrolDirection.x = -App.M.Character.BumpingDirection;

            //If we're patrolling, move the enemy.
            App.M.Character.SetVelocity(_patrolDirection);
        }

        public override bool CheckPrerequisite()
        {
            return false;
        }
    }
}