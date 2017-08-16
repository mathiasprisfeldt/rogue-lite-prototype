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
            if (IsActive && App.M.Target)
            {
                Vector2 dir = App.M.Target.M.ActionController.Rigidbody.position - App.M.Character.Rigidbody.position;
                dir.Normalize();
                App.M.Character.SetVelocity(new Vector2(Mathf.Round(dir.x), 0));
            }
        }

        public override bool CheckPrerequisite()
        {
            return App.M.Target && App.C.CurrentState is EnemyPatrol && !(App.C.CurrentState is EnemyAttack);
        }

        public override void StateUpdate()
        {
            if (!App.M.Target)
                App.C.ResetToInitial();
        }
    }
}