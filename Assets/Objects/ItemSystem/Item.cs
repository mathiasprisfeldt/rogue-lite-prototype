using Archon.SwissArmyLib.Events;
using Health;
using UnityEngine;
using Timer = AcrylecSkeleton.Utilities.Timer;

namespace ItemSystem
{
    public enum ItemType
    {
        Passive,
        Active
    }

    /// <summary>
    /// Purpose: Base class for all items.
    /// Creator: MP
    /// </summary>
    public abstract class Item : MonoBehaviour
    {
        [SerializeField, Tooltip("Timer for cooldown on activation.")]
        private Timer _timer;

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

        void Start()
        {
            _timer.FinishedEvent.AddListener(() =>
            {
                IsActivationReady = true;
            });
        }

        /// <summary>
        /// Triggers item activation, and sets the item on cooldown.
        /// </summary>
        public void Activate()
        {
            if (IsActivationReady)
            {
                OnActivated();
                IsActivationReady = false;
                _timer.StartTimer();
            }
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
        public virtual void OnHit(HealthController victim) { }

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

        protected virtual void OnDestroy()
        {
            Remove();
        }
    }
}