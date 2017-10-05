using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ItemSystem.UI
{
    /// <summary>
    /// Purpose:
    /// Creator: MP
    /// </summary>
    public class ItemStealIcon : MonoBehaviour 
    {
        [SerializeField]
        private TextMeshProUGUI _name;

        [SerializeField]
        private TextMeshProUGUI _desc;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private Image _inputIcon;

        [SerializeField]
        private bool _isLeft;

        public Item Item { get; set; }

        public void SetItem(Item item)
        {
            _name.text = item.Name;
            _desc.text = item.Description;
            _icon.sprite = item.Icon;
            Item = item;
        }
    }
}