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
        public override bool ShouldChange()
        {
            return false;
        }
    }
}