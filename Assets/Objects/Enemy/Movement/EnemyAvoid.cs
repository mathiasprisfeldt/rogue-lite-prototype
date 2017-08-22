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
        private float avoidDistance = 2;

        public override void StateUpdate()
        {
            base.StateUpdate();

            if (CheckPrerequisite() && App.M.Character.BumpingDirection == 0)
                App.C.SetVelocity(new Vector2(-Mathf.Round(App.C.ToPlayer.normalized.x), 0));
            else
                App.C.ChangeState(null);
        }

        public override bool CheckPrerequisite()
        {
            return App.C.ToPlayer.magnitude < avoidDistance;
        }
    }
}