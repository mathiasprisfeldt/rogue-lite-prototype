using System;
using System.Collections.Generic;
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
        [SerializeField]
        private Character _owner;

        [SerializeField]
        private List<Item> _items;

        public Character Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public List<Item> Items
        {
            get { return _items; }
            set { _items = value; }
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
            List<Item> newItems = new List<Item>();

            foreach (Item starterItem in Items)
            {
                Item newItem = Instantiate(starterItem, transform);
                newItem.ItemHandler = this;

                newItems.Add(newItem);
            }

            Items.Clear();
            Items = newItems;
        }

        /// <summary>
        /// Called from healthcontroller when it needs to check damage dealer for OnHit specific items.
        /// </summary>
        public void OnHit(HealthController healthController)
        {
            foreach (Item item in Items)
            {
                item.OnHit(healthController);
            }
        }
    }
}