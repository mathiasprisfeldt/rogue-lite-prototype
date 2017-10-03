using Health;
using InControl;
using Managers;
using RogueLiteInput;
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
        [SerializeField]
        private Timer _cooldownTimer;

        [SerializeField]
        private ItemType _type;

        public ProxyPlayerAction ActivationAction { get; set; }
        public ItemHandler ItemHandler { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Sprite Icon { get; set; }

        public ItemType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public bool IsActivationReady
        {
            get { return !CooldownTimer.IsRunning; }
        }

        public Timer CooldownTimer
        {
            get { return _cooldownTimer; }
        }

        void Start()
        {
            if (CooldownTimer != null)
            {
                CooldownTimer.Finished.AddListener(OnCooldownFinished);
                CooldownTimer.Elapsed.AddListener(OnCooldownElapsed);
            }
        }

        /// <summary>
        /// Triggers item activation, and sets the item on cooldown.
        /// </summary>
        public void Activate()
        {
            if (IsActivationReady)
            {
                OnActivated();
                CooldownTimer.StartTimer();
            }
        }

        public void Remove()
        {
            //If we havent removed this Item from its ItemHandler do so.
            if (ItemHandler && ItemHandler.Items.Contains(this))
                ItemHandler.Items.Remove(this);
        }

        /// <summary>
        /// Called when this character deals damage to another character.
        /// </summary>
        public virtual void OnHit(HealthController victim) { }

        /// <summary>
        /// Called when an item gets equipped.
        /// </summary>
        public virtual void OnEquipped()
        {
            ItemHandler.ItemEquipped.Invoke(this);
        }

        /// <summary>
        /// Called when an item gets unequipped.
        /// </summary>
        public virtual void OnUnEquipped()
        {
            ItemHandler.ItemUnEquipped.Invoke(this);
        }

        /// <summary>
        /// Called when Item is activated.
        /// </summary>
        public virtual void OnActivated() { }

        /// <summary>
        /// Called when cooldown timer is finished.
        /// </summary>
        protected virtual void OnCooldownFinished() { }

        /// <summary>
        /// Called when cooldown timer has its elapsed triggered.
        /// </summary>
        protected virtual void OnCooldownElapsed() { }

        protected virtual void OnDestroy()
        {
            Remove();
            CooldownTimer.Destroy();
        }
    }
}