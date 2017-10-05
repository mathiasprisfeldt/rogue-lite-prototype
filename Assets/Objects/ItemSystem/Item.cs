using Health;
using RogueLiteInput;
using UnityEngine;
using Timer = AcrylecSkeleton.Utilities.Timer;

namespace ItemSystem
{
    public enum ItemType
    {
        None,
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

        [SerializeField]
        private Sprite _icon;

        public ProxyPlayerAction ActivationAction { get; set; }
        public ItemHandler ItemHandler { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Sprite Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

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

        /// <summary>
        /// Removes this item from its itemhandler.
        /// </summary>
        /// <param name="replacement">This item will be the replacement in the item handler.</param>
        public void RemoveSelf(Item replacement = null)
        {
            if (!ItemHandler)
                return;

            var ourself = ItemHandler.Items.Find(this);

            //If we havent removed this Item from its ItemHandler do so.
            if (ourself != null)
            {
                OnUnEquipped();

                if (replacement)
                {
                    replacement.RemoveSelf();
                    ourself.Value = replacement;
                }
                else
                {
                    ItemHandler.Items.Remove(ourself);
                }
            }
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
            if (ItemHandler && ItemHandler.ItemEquipped != null)
                ItemHandler.ItemEquipped.Invoke(this);
        }

        /// <summary>
        /// Called when an item gets unequipped.
        /// </summary>
        public virtual void OnUnEquipped()
        {
            if (ItemHandler && ItemHandler.ItemUnEquipped != null)
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
            RemoveSelf();
            CooldownTimer.Destroy();
        }
    }
}