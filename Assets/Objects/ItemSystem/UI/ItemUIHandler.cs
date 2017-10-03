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
#pragma warning restore 649

    void Awake()
    {
        _itemHandler.ItemEquipped.AddListener(this);
        _itemHandler.ItemUnEquipped.AddListener(this);
    }

    public void OnEvent(int eventId, Item item)
    {
        switch (eventId)
        {
            case ItemHandler.ON_ITEM_EQUIPPED:

                PlayerUIItem newUiItem = Instantiate(_playerUiItemPrefab, _playerUiItemParent);
                newUiItem.Item = item;
                _playerUiItems.AddLast(newUiItem);

                break;
            case ItemHandler.ON_ITEM_UNEQUIPPED:

                PlayerUIItem foundItem = _playerUiItems.FirstOrDefault(uiItem => uiItem.Item == item);
                _playerUiItems.Remove(foundItem);
                Destroy(foundItem);

                break;
        }
    }
}
