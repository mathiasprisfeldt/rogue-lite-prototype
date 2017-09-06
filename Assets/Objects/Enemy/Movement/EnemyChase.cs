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
            if (!IsActive)
                return;

            if (Context.C.Target && 
                Context.M.Character.BumpingDirection == 0 &&
                Context.C.ToPlayer.magnitude > 1f)
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

            if (!Context.C.Target)
                ChangeState<EnemyIdle>();
        }

        public override bool ShouldTakeover()
        {
            if (Context.C.Target && 
                !IsState<EnemyAttack>() &&
                !IsState<EnemyAvoid>())
                return true;

            return false;
        }
    }
}