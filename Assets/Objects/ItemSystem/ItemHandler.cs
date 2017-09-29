using System.Collections.Generic;
using System.Linq;
using Archon.SwissArmyLib.Events;
using Controllers;
using Health;
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

                Item newItem = Instantiate(starterItem, transform);
                newItem.ItemHandler = this;

                Items.AddFirst(newItem);
                newItem.OnEquipped();
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
        public void Steal(ItemHandler victim)
        {
            foreach (Item victimItem in victim.Items.ToList())
            {
                Steal(null, victimItem);
            }
        }

        /// <summary>
        /// Steals specific item from an ItemHandler and replaces it.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="new"></param>
        public void Steal(Item current, Item @new)
        {
            if (current)
            {
                current.OnUnEquipped();

                //TODO: Drop the item on the floor
                //Meanwhile just destroy it.
                Destroy(current);
            }

            if (@new)
            {
                @new.Remove();

                @new.ItemHandler = this;
                @new.OnEquipped();
                @new.transform.SetParent(transform);
            }
        }

        /// <summary>
        /// Swaps an item on itself with another one.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Swap(Item a, Item b)
        {

        }

        public bool CanStealFrom()
        {
            return Owner.HealthController.IsDead && Items.Any();
        }
    }
}