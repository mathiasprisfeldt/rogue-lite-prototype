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
            base.Begin();
        }

        public override void Think(float deltaTime)
        {
            int bumpingDirection = Context.M.Character.BumpingDirection;
            int lookDir = Context.M.Character.LookDirection;

            //If we're bumping into something, change direction.
            if (bumpingDirection == -1 &&  lookDir == -1)
                _patrolDirection.x = 1;
            else if (bumpingDirection == 1 && lookDir == 1)
                _patrolDirection.x = -1;
            else
                _patrolDirection.x = lookDir;
        }

        public override bool ShouldTakeover()
        {
            if (IsState<EnemyIdle>())
                return true;

            return false;
        }

        void FixedUpdate()
        {
            //If we're patrolling, move the enemy.
            if (IsActive && Context.M.Character.OnGround)
            {
                Context.C.Move(_patrolDirection, forceTurn: true);
            }
        }
    }
}