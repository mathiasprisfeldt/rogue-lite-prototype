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
            if (IsActive && Context.M.Target)
            {
                if (Context.M.Character.OnGround && !Context.M.IsFlying)
                    Context.C.Move(Mathf.Round(Context.C.ToPlayer.normalized.x) * Vector2.right, forceTurn: true);
                else if (Context.M.IsFlying)
                    Context.C.Move(Context.C.ToPlayer.normalized, true, true);
            }
        }

        public override void Reason()
        {
            base.Reason();

            if (!Context.M.Target)
                ChangeState<EnemyIdle>();
        }

        public override bool ShouldTakeover()
        {
            if (Context.M.Target && !IsState<EnemyAttack>())
                return true;

            return false;
        }
    }
}