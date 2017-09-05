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
        private Vector2 _lastTurnPosition;
        private float _turnPosDistance; //Distance between last turn pos and current pos when turning.

        private Vector2 _homePoint;
        private Vector2 _patrolDirection = Vector2.right;

        [SerializeField]
        private float _homeDistance = 5;

        [SerializeField]
        private float _giveUpThreshhold = .5f;

        public override void Begin()
        {
            _turnPosDistance = _giveUpThreshhold*2;
            _lastTurnPosition = Vector2.zero;

            _homePoint = Context.M.Character.Rigidbody.position;
            _patrolDirection = new Vector2(Context.M.Character.LookDirection, 0);
            base.Begin();
        }

        public override void Act(float deltaTime)
        {
            if (Context.C.IsTurning)
                return;

            Vector2 plyPos = Context.M.Character.Rigidbody.position;
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
                Vector2.Distance(_homePoint, plyPos) > _homeDistance)
                targetDirection = (_homePoint - plyPos).normalized;

            //Check distance from current pos to last turn pos.
            if (_patrolDirection != targetDirection && _turnPosDistance > _giveUpThreshhold)
            {
                if (_lastTurnPosition != Vector2.zero)
                {
                    float tempDist = Vector2.Distance(_lastTurnPosition, plyPos);

                    if (tempDist < _turnPosDistance)
                        _turnPosDistance = tempDist;
                }

                _lastTurnPosition = plyPos;
            }

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
            if (IsActive && _turnPosDistance >= _giveUpThreshhold)
            {
                if (Context.M.Character.OnGround)
                    Context.C.Move(_patrolDirection, forceTurn: true);
                else if (Context.M.Character.IsFlying)
                    Context.C.Move(_patrolDirection, true, true);
            }
        }
    }
}