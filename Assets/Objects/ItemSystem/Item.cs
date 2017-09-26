using Archon.SwissArmyLib.Events;
using Controllers;
using Health;
using UnityEngine;
using UnityEngine.UI;

namespace ItemSystem
{
    public enum ItemType
    {
        Passive,
        Ability
    }

    /// <summary>
    /// Purpose: Base class for all items.
    /// Creator: MP
    /// </summary>
    public abstract class Item : MonoBehaviour, TellMeWhen.ITimerCallback
    {
        [SerializeField, Tooltip("Cooldown duration when activating the item.")]
        private float _activationCdDuration;

        [SerializeField]
        private ItemType _type;

        public bool IsActivationReady { get; private set; }

        public ItemHandler ItemHandler { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Sprite Icon { get; set; }

        public ItemType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public void Remove()
        {
            //If we havent removed this Item from its ItemHandler do so.
            if (ItemHandler.Items.Contains(this))
                ItemHandler.Items.Remove(this);
        }

        /// <summary>
        /// Called when this character deals damage to another character.
        /// </summary>
        public virtual void Hit(HealthController victim) { }

        /// <summary>
        /// Called when an item gets equipped.
        /// </summary>
        public virtual void OnEquipped() { }

        /// <summary>
        /// Called when an item gets unequipped.
        /// </summary>
        public virtual void OnUnEquipped() { }

        /// <summary>
        /// Called when Item is activated.
        /// </summary>
        public virtual void OnActivated() { }

        /// <summary>
        /// Triggers item activation, and sets the item on cooldown.
        /// </summary>
        public void Activate()
        {
            if (IsActivationReady)
            {
                OnActivated();
                TellMeWhen.Seconds(_activationCdDuration, this);
                IsActivationReady = false;
            }
        }

        protected virtual void OnDestroy()
        {
            Remove();
        }

        public void OnTimesUp(int id, object args)
        {
            //We're no longer on cooldown.
            IsActivationReady = true;
        }
    }
}