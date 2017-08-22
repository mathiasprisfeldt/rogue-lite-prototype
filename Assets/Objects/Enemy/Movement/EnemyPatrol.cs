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

        public override void Begin()
        {
            _patrolDirection = new Vector2(Context.M.Character.LookDirection, 0);
        }

        public override void Think(float deltaTime)
        {
            int bumpingDirection = Context.M.Character.BumpingDirection;

            //If we're bumping into something, change direction.
            if (bumpingDirection != 0)
                _patrolDirection.x = -Context.M.Character.BumpingDirection;
            else
                _patrolDirection.x = Context.M.Character.LookDirection;
        }

        public override bool ShouldChange()
        {
            if (IsState<EnemyIdle>())
                return true;

            return false;
        }

        void FixedUpdate()
        {
            //If we're patrolling, move the enemy.
            if (IsActive && Context.M.Character.OnGround)
                Context.C.SetVelocity(_patrolDirection, forceTurn: true);
        }
    }
}