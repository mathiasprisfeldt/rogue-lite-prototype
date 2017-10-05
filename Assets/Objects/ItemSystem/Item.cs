using Health;
using InControl;
using Managers;
using RogueLiteInput;
using System;
using System.Linq;
using UnityEngine;
using Timer = AcrylecSkeleton.Utilities.Timer;

namespace ItemSystem
{
    public enum ItemType { Passive, Active }
    public enum ItemSlave { Master, Slave }

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

        protected ItemSlave _slaveState;

        public ProxyPlayerAction ActivationAction { get; set; }
        public ItemHandler ItemHandler { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Sprite Icon { get; set; }
        public Item Other { get; set; }

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


            foreach (var item in ItemHandler.Items)
            {
                if (item != this && item.GetType() == GetType())
                {
                    item.Other = this;
                    item._slaveState = ItemSlave.Master;
                    _slaveState = ItemSlave.Slave;
                    item.DoubleUp();
                }
            }
        }

        protected virtual void DoubleUp() { }

        protected virtual void DoubleDown() { }

        /// <summary>
        /// Called when an item gets unequipped.
        /// </summary>
        public virtual void OnUnEquipped()
        {
            ItemHandler.ItemUnEquipped.Invoke(this);

            if (Other)
                Other.DoubleDown();
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