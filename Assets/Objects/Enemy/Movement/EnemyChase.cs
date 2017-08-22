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
            if (IsActive && App.M.Target && App.M.Character.OnGround)
                App.C.SetVelocity(new Vector2(Mathf.Round(App.C.ToPlayer.normalized.x), 0));
        }

        public override bool CheckPrerequisite()
        {
            return App.M.Target && App.C.CurrentState is EnemyPatrol && !(App.C.CurrentState is EnemyAttack) || App.C.CurrentState == null;
        }

        public override void StateUpdate()
        {
            if (!App.M.Target)
                App.C.ResetToInitial();
        }
    }
}