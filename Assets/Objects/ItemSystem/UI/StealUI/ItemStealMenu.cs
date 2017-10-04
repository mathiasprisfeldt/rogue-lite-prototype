using System.Linq;
using AcrylecSkeleton.Managers;
using UnityEngine;

namespace ItemSystem.UI
{
    /// <summary>
    /// Purpose:
    /// Creator: MP
    /// </summary>
    public class ItemStealMenu : MonoBehaviour
    {
        [SerializeField]
        private ItemStealIcon _left;

        [SerializeField]
        private ItemStealIcon _right;

        [SerializeField]
        private ItemStealIcon _new;

        /// <summary>
        /// Sets up 
        /// </summary>
        /// <param name="newItem"></param>
        public void Initialize(Item newItem)
        {
            TimeManager.Instance.IsPaused = true;

            _new.SetItem(newItem);
        }
    }
}