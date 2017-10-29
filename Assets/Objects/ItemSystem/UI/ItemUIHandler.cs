using System;
using System.Collections.Generic;
using System.Linq;
using Archon.SwissArmyLib.Events;
using ItemSystem;
using UnityEngine;

/// <summary>
/// Used to hold and handle all player UI items for items.
/// </summary>
public class ItemUIHandler : MonoBehaviour, IEventListener<Item>
{ 
    private readonly LinkedList<PlayerUIItem> _playerUiItems = new LinkedList<PlayerUIItem>();

#pragma warning disable 649
    [SerializeField]
    private ItemHandler _itemHandler;

    [SerializeField]
    private PlayerUIItem _playerUiItemPrefab;

    [SerializeField]
    private Transform _playerUiItemParent;

    [SerializeField]
    private List<ItemInputImage> _inputImages;
#pragma warning restore 649

    public List<ItemInputImage> InputImages
    {
        get { return _inputImages; }
    }

    void Awake()
    {
        _itemHandler.ItemEquipped.AddListener(this);
        _itemHandler.ItemUnEquipped.AddListener(this);

        for (int i = 0; i < _itemHandler.MaxActives + _itemHandler.MaxPassives; i++)
        {
            PlayerUIItem newUiItem = Instantiate(_playerUiItemPrefab, _playerUiItemParent);
            newUiItem.gameObject.SetActive(true);
            _playerUiItems.AddLast(newUiItem);

            newUiItem.Owner = this;
            newUiItem.ItemType = i < _itemHandler.MaxActives ? ItemType.Active : ItemType.Passive;
        }
    }

    public void OnEvent(int eventId, Item item)
    {
        switch (eventId)
        {
            case ItemHandler.ON_ITEM_EQUIPPED:
                PlayerUIItem unusedItemIcon = _playerUiItems.FirstOrDefault(uiItem => uiItem.ItemType == item.Type && 
                    !uiItem.Item && 
                    !_playerUiItems.Select(playerUiItem => playerUiItem.Item).Contains(item));

                if (unusedItemIcon)
                {
                    unusedItemIcon.SetItem(item);
                }
                break;
            case ItemHandler.ON_ITEM_UNEQUIPPED:
                PlayerUIItem foundItem = _playerUiItems.FirstOrDefault(uiItem => uiItem.Item == item);
                if (foundItem)
                    foundItem.SetItem(null);
                break;
        }
    }

    [Serializable]
    public class ItemInputImage
    {
        public string Name;
        public Sprite KeyboardSprite;
        public Sprite ControllerSprite;
    }
}
