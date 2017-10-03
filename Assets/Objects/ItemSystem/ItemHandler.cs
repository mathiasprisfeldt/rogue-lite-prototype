using System;
using System.Collections.Generic;
using System.Linq;
using Archon.SwissArmyLib.Events;
using Controllers;
using Health;
using Managers;
using RogueLiteInput;
using UnityEngine;

namespace ItemSystem
{
    /// <summary>
    /// Purpose: Handles a characters items.
    /// Creator: MP
    /// </summary>
    public class ItemHandler : MonoBehaviour
    {
        public const int
            ON_ITEM_UNEQUIPPED = 0,
            ON_ITEM_EQUIPPED   = 1;

        [SerializeField]
        private Character _owner;

        [SerializeField]
        private List<Item> _itemsAtStart;

        [SerializeField]
        private Vector2 _popupOffset;

        [SerializeField]
        private int _maxPassives = 2;

        [SerializeField]
        private int _maxActives = 2;

        public LinkedList<Item> Items { get; set; }

        public Character Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public Vector2 PopupOffset
        {
            get { return _popupOffset; }
            set { _popupOffset = value; }
        }

        public readonly Event<Item> ItemEquipped = new Event<Item>(ON_ITEM_EQUIPPED);
        public readonly Event<Item> ItemUnEquipped = new Event<Item>(ON_ITEM_UNEQUIPPED);

        public bool CanStealFrom
        {
            get
            {
                return Owner.HealthController.IsDead && Items.Any();
            }
        }

        public int MaxPassives
        {
            get { return _maxPassives; }
        }

        public int MaxActives
        {
            get { return _maxActives; }
        }

        void Start()
        {
            SetupStarterItems();
        }

        /// <summary>
        /// Used to instantiate items that are serialized with the item handler.
        /// </summary>
        public void SetupStarterItems()
        {
            Items = new LinkedList<Item>();

            foreach (Item starterItem in _itemsAtStart)
            {
                if (!starterItem)
                    continue;

                Item newItem = Instantiate(starterItem);
                AddItem(newItem);
            }
        }

        /// <summary>
        /// Called from healthcontroller when it needs to check damage dealer for Hit specific items.
        /// </summary>
        public void OnHit(HealthController healthController)
        {
            foreach (Item item in Items)
                item.OnHit(healthController);
        }

        /// <summary>
        /// Steals all items from another item handler
        /// </summary>
        /// <param name="victim">The item handler to steal from</param>
        public bool Steal(ItemHandler victim)
        {
            if (!victim.Items.Any())
                return false;

            bool success = true;

            foreach (Item victimItem in victim.Items.ToList())
            {
                success &= Steal(null, victimItem);
            }

            return success;
        }

        /// <summary>
        /// Steals specific item from an ItemHandler and replaces it.
        /// </summary>
        public bool Steal(Item current, Item newItem)
        {
            if (current)
            {
                current.OnUnEquipped();

                //TODO: Drop the item on the floor
                //Meanwhile just destroy it.
                Destroy(current);
            }

            return AddItem(newItem);
        }

        /// <summary>
        /// Swaps an item on itself with another one.
        /// </summary>
        public void Swap(Item a, Item b)
        {

        }

        /// <summary>
        /// Adds a new item to this itemhandler.
        /// </summary>
        public bool AddItem(Item newItem)
        {
            if (!newItem)
                return false;

            if (!CanCarry(newItem.Type))
                return false;

            newItem.Remove();
            Items.AddFirst(newItem);

            //Find a activationaction for the new item.
            ProxyInputActions inputActions = GameManager.Instance.Player.C.PlayerActions.ProxyInputActions;
            bool isSpecial1Occupied = Items.Any(item => item.ActivationAction == inputActions.Special1);
            newItem.ActivationAction = isSpecial1Occupied ? inputActions.Special2 : inputActions.Special1;

            newItem.ItemHandler = this;
            newItem.OnEquipped();
            newItem.transform.SetParent(transform);

            return true;
        }

        /// <summary>
        /// Checks ifthis item handler can carry anymore of target item type.
        /// It uses the condition fields for checking.
        /// </summary>
        public bool CanCarry(ItemType type)
        {
            int matchingItemCount = Items.Count(item => item.Type == type);

            switch (type)
            {
                case ItemType.Passive:
                    return matchingItemCount < MaxPassives;
                case ItemType.Active:
                    return matchingItemCount < MaxActives;
                default:
                    return false;
            }
        }
    }
}