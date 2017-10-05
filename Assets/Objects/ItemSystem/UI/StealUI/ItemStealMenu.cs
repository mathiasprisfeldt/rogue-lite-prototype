using System.Linq;
using AcrylecSkeleton.Managers;
using Managers;
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
            if (GameManager.Instance.Player.C.PlayerActions.Interact.WasPressed)
            {
                Close(true);
            }

            Item oldItem = null;
            ItemStealIcon oldItemIcon = null;

            if (_left.Item.ActivationAction.Action.WasPressed)
            {
                oldItem = _left.Item;
                oldItemIcon = _left;
            }

            if (_right.Item.ActivationAction.Action.WasPressed)
            {
                oldItem = _right.Item;
                oldItemIcon = _right;
            }

            if (oldItem && oldItem.GetType() != _new.Item.GetType())
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

            var items = owner.Items.Where(item => item.Type == newItem.Type).ToList();

            if (items[1])
                _left.SetItem(items[1]);

            if (items[0])
                _right.SetItem(items[0]);
            else
                _right.gameObject.SetActive(false);

            _new.SetItem(newItem);
        }

        /// <summary>
        /// Closes the steal menu
        /// </summary>
        public void Close(bool applyLoadout = false)
        {
                if (applyLoadout)
            {
                if (_left && _left.Item)
                    _context.Steal(1, _left.Item);

                if (_right && _right.Item)
                    _context.Steal(0, _right.Item);
            }

            if (_context)
                _context.Character.LockMovement = false;

            Destroy(gameObject);
        }
    }
}