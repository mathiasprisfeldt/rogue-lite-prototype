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
            else
                _patrolDirection.x = App.M.Character.LookDirection;
        }

        void FixedUpdate()
        {
            //If we're patrolling, move the enemy.
            if (IsActive && App.M.Character.OnGround)
                App.C.SetVelocity(_patrolDirection, forceTurn: true);
        }

        public override bool CheckPrerequisite()
        {
            return !App.C.CurrentState;
        }
    }
}