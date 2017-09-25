using Health;
using UnityEngine;

namespace ItemSystem
{
    /// <summary>
    /// Purpose: Base class for all items.
    /// Creator: MP
    /// </summary>
    public abstract class Item : MonoBehaviour 
    {
        public ItemHandler ItemHandler { get; set; }

        /// <summary>
        /// Called when this character deals damage to another character.
        /// </summary>
        public abstract void OnHit(HealthController victim);
    }
}