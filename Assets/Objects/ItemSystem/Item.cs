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
        [SerializeField]
        private Timer _cooldownTimer;

        [SerializeField]
        private ItemType _type;

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
            get { return !_cooldownTimer.IsRunning; }
        }

        void Start()
        {
            if (_cooldownTimer != null)
            {
                _cooldownTimer.Finished.AddListener(OnCooldownFinished);
                _cooldownTimer.Elapsed.AddListener(OnCooldownElapsed);
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
                _cooldownTimer.StartTimer();
            }
        }

        public void Remove()
        {
            //If we havent removed this Item from its ItemHandler do so.
            if (ItemHandler.Items.Contains(this))
                ItemHandler.Items.Remove(this);

            _cooldownTimer.Destroy();
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
        }
    }
}