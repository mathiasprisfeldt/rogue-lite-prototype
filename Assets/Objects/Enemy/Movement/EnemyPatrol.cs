using AcrylecSkeleton.Utilities;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose: Patrol pattern for all enemies patrolling an area
    /// Creator: MP
    /// </summary>
    public class EnemyPatrol : EnemyState
    {
        private Vector2 _homePoint;
        private Vector2 _patrolDirection = Vector2.right;

        [SerializeField]
        private float _homeDistance = 5;

        public override void Begin()
        {
            _homePoint = Context.M.Character.Rigidbody.position;
            _patrolDirection = new Vector2(Context.M.Character.LookDirection, 0);
            base.Begin();
        }

        public override void Act(float deltaTime)
        {
            Vector2 targetDirection = _patrolDirection;

            int bumpingDirection = Context.M.Character.BumpingDirection;
            int lookDir = Context.M.Character.LookDirection;

            //If we're bumping into something, change direction.
            if (bumpingDirection == -1 &&  lookDir == -1)
                targetDirection.x = 1;
            else if (bumpingDirection == 1 && lookDir == 1)
                targetDirection.x = -1;
            else
                targetDirection.x = lookDir;

            //If the enemy gets too far out, change its direction.
            if (!_homeDistance.FastApproximately(0) && 
                Vector2.Distance(_homePoint, Context.M.Character.Rigidbody.position) > _homeDistance)
                targetDirection = (_homePoint - Context.M.Character.Rigidbody.position).normalized;

            _patrolDirection = targetDirection;
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
            if (IsActive)
            {
                if (Context.M.Character.OnGround)
                    Context.C.Move(_patrolDirection, forceTurn: true);
                else if (Context.M.Character.IsFlying)
                    Context.C.Move(_patrolDirection, true, true);
            }
        }
    }
}