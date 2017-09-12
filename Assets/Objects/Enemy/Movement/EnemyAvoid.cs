using AcrylecSkeleton.Utilities;
using UnityEngine;
using Archon.SwissArmyLib.Automata;

namespace Enemy
{
    /// <summary>
    /// Purpose: Tries to keep a certain distance from the target.
    /// Creator: MP
    /// </summary>
    public class EnemyAvoid : EnemyState 
    {
        [SerializeField]
        private float _avoidDistance = 2;

        void FixedUpdate()
        {
            if (IsActive)
            {
                float dir = 0;

                if (Context.C.ToPlayer.magnitude < _avoidDistance)
                    dir = -Mathf.Round(Context.C.ToPlayer.normalized.x);

                if (!dir.FastApproximately(Context.M.Character.BumpingDirection))
                    Context.C.Move(dir * Vector2.right);
                else
                    Context.M.Character.StandStill();

                if (Context.M.CanBackPaddle && Context.C.IsTargetBehind)
                    Context.C.Turn();
            }
        }

        public override bool ShouldTakeover()
        {
            if (Context.C.Target && 
                Context.M.Character.BumpingDirection == 0 && 
                Context.C.ToPlayer.magnitude < _avoidDistance)
                return true;

            return false;
        }

        public override void Reason()
        {
            if (!Context.C.Target)
            {
                ChangeState<EnemyIdle>();
                return;
            }

            base.Reason();
        }
    }
}