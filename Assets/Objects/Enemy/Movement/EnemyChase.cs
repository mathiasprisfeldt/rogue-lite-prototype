using AcrylecSkeleton.Extensions;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose: Chases the player.
    /// Creator: MP
    /// </summary>
    public class EnemyChase : EnemyState
    {
        void FixedUpdate()
        {
            if (IsActive && Context.M.Target && Context.M.Character.BumpingDirection == 0)
            {
                if (Context.M.Character.OnGround && !Context.M.Character.IsFlying)
                    Context.C.Move(Mathf.Round(Context.C.ToPlayer.normalized.x) * Vector2.right, forceTurn: true);
                else if (Context.M.Character.IsFlying)
                    Context.C.Move(Context.C.ToPlayer.normalized, true, true);
            }
            else
                Context.M.Character.StandStill();
        }

        public override void Reason()
        {
            base.Reason();

            if (!Context.M.Target)
                ChangeState<EnemyIdle>();
        }

        public override bool ShouldTakeover()
        {
            if (Context.M.Target && 
                !IsState<EnemyAttack>())
                return true;

            return false;
        }
    }
}