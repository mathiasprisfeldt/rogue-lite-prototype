using Controllers;
using Health;
using UnityEngine;
using UnityEngine.UI;

namespace ItemSystem
{
    /// <summary>
    /// Purpose: Base class for all items.
    /// Creator: MP
    /// </summary>
    public abstract class Item : MonoBehaviour
    {
        public ItemHandler ItemHandler { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Image Icon { get; set; }

        /// <summary>
        /// Called when this character deals damage to another character.
        /// </summary>
        public abstract void OnHit(HealthController victim);
    }
}