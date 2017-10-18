﻿using System.Linq;
using AcrylecSkeleton.Managers;
using Managers;
using RogueLiteInput;
using UnityEngine;

namespace ItemSystem.UI
{
    /// <summary>
    /// Purpose:
    /// Creator: MP
    /// </summary>
    public class ItemStealMenu : MonoBehaviour
    {
        private ItemHandler _context;

        [SerializeField]
        private ItemStealIcon _left;

        [SerializeField]
        private ItemStealIcon _right;

        [SerializeField]
        private ItemStealIcon _new;

        void Update()
        {
            if (GameManager.Instance.Player.C.PlayerActions.Jump.WasPressed ||
                Input.GetKeyDown(KeyCode.Escape))
            {
                Close(true);
            }

            Item oldItem = null;
            ItemStealIcon oldItemIcon = null;

            if (GameManager.Instance.Player.C.PlayerActions.ProxyInputActions.Special1.Action.WasPressed)
            {
                oldItem = _left.Item;
                oldItemIcon = _left;
            }

            if (GameManager.Instance.Player.C.PlayerActions.ProxyInputActions.Special2.Action.WasPressed)
            {
                oldItem = _right.Item;
                oldItemIcon = _right;
            }

            if (oldItem)
            {
                var newItem = _new.Item;
                oldItemIcon.SetItem(newItem);
                _new.SetItem(oldItem);
            }
        }

        /// <summary>
        /// Sets up the steal menu
        /// </summary>
        public void Initialize(ItemHandler owner, Item newItem)
        {
            _context = owner;
            _context.Character.LockMovement = true;
            _context.Character.Rigidbody.velocity = Vector2.zero;

            var items = owner.Items.Where(item => item.Type == newItem.Type).ToList();

            if (items[1])
                _left.SetItem(items[1]);

            if (items[0])
                _right.SetItem(items[0]);
            else
                _right.gameObject.SetActive(false);

            _new.SetItem(newItem);
            if (LevelManager.Instance)
                LevelManager.Instance.EscapeIsActive = false;
        }

        /// <summary>
        /// Closes the steal menu
        /// </summary>
        public void Close(bool applyLoadout = false)
        {
            if (applyLoadout)
            {
                if (_left && _left.Item)
                    _context.Steal(1, _left.Item, false);

                if (_right && _right.Item)
                    _context.Steal(0, _right.Item, false);

                Destroy(_new.Item.gameObject);
            }

            if (_context)
                _context.Character.LockMovement = false;

            if (LevelManager.Instance)
                LevelManager.Instance.EscapeIsActive = true;

            Destroy(gameObject);
        }
    }
}