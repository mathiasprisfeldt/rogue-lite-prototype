using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Purpose: Default state for all enemies
    /// Creator: MP
    /// </summary>
    [DisallowMultipleComponent]
    public class EnemyIdle : EnemyState
    {
        public override void Begin()
        {
            Context.M.Character.StandStill();
            base.Begin();
        }

        public override bool ShouldChange()
        {
            return false;
        }
    }
}