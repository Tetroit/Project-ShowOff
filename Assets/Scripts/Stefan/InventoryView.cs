using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryItem : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite InventorySprite { get; private set; }

}
public class InventoryItemView : MonoBehaviour
{
    public InventoryItem Item => _item;
    InventoryItem _item;

    [SerializeField] Image _itemImage;

    public void SetItem(InventoryItem item)
    {
        _item = item;

        _itemImage.sprite = _item.InventorySprite;
    }

    public void Select()
    {

    }

    public void Deselect()
    {

    }
}

public class InventoryView : MonoBehaviour
{
    public UnityEvent<InventoryItem> OnItemSelect;
    public UnityEvent<InventoryItem> OnItemDeselect;
    [SerializeField] InventoryItemView _itemSlotPrefab;
    [SerializeField] Transform _itemsContainer;
    List<InventoryItemView> _items = new();

    InventoryItem _currentItem;

    void Awake()
    {
        if (_itemsContainer == null) _itemsContainer = transform;
    }

    public void UpdateUI(InventoryItem[] items)
    {
        foreach (InventoryItem item in items)
        {
            InventoryItemView instance = Instantiate(_itemSlotPrefab, _itemsContainer);
            instance.SetItem(item);
            _items.Add(instance);
        }
            
    }

}
