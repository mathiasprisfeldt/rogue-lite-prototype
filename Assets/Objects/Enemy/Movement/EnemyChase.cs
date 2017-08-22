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
            if (IsActive && Context.M.Target && Context.M.Character.OnGround)
                Context.C.SetVelocity(new Vector2(Mathf.Round(Context.C.ToPlayer.normalized.x), 0), forceTurn: true);
        }

        public override void Reason()
        {
            base.Reason();

            if (!Context.M.Target)
                ChangeState<EnemyIdle>();
        }

        public override bool ShouldChange()
        {
            if (Context.M.Target && !IsState<EnemyAttack>())
                return true;

            return false;
        }
    }
}