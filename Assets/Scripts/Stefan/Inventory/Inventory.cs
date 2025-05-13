using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

[CreateAssetMenu(menuName = "Stefan/Inventory")]
public class Inventory : ScriptableObject
{
    [SerializeField] List<InventoryItem> _items;
    public ReadOnlyCollection<InventoryItem> Items => new(_items);

    public void AddItem(InventoryItem inventoryItem)
    {
        _items.Add(inventoryItem);
    }

    public void RemoveItem(InventoryItem inventoryItem)
    {
        _items.Remove(inventoryItem);

    }
}
