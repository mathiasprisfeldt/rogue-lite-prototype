using AcrylecSkeleton.StateKit;
using CharacterController;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose: Patrol pattern for all enemies patrolling an area
    /// Creator: MP
    /// </summary>
    public class EnemyPatrol : EnemyState
    {
        private Vector2 _patrolDirection = Vector2.right;

        public override void StateStart()
        {
            base.StateStart();

            _patrolDirection = new Vector2(App.M.Character.LookDirection, 0);
        }

        public override void StateUpdate()
        {
            int bumpingDirection = App.M.Character.BumpingDirection;

            //If we're bumping into something, change direction.
            if (bumpingDirection != 0)
                _patrolDirection.x = -App.M.Character.BumpingDirection;
        }

        void FixedUpdate()
        {
            //If we're patrolling, move the enemy.
            if (IsActive)
                App.M.Character.SetVelocity(_patrolDirection);
        }

        public override bool CheckPrerequisite()
        {
            return false;
        }
    }
}