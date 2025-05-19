using System.Collections.Generic;
using UnityEngine;

public class KeySlotsInventoryView : InventoryView
{
    [SerializeField] Transform _itemsContainer;

    protected override void OnAwake()
    {
        if (_itemsContainer == null) _itemsContainer = transform;
    
    }

    //need to disable controlls? item interaction
    void OnEnable()
    {
        ItemSelected.AddListener(OnSelect);

    }

    void OnDisable()
    {
        ItemSelected.RemoveListener(OnSelect);

    }

    public override void UpdateUI(IEnumerable<InventoryItem> items)
    {
        this.items.Clear();
        int i = 0;
        foreach (InventoryItem item in items)
        {
            if (i++ >= capacity) break;

            InventoryItemView instance = Instantiate(item.SelectionBehavior, _itemsContainer);
            instance.SetItem(item);
            this.items.Add(instance);
        }
    }

    void OnSelect(InventoryItemView item)
    {
        StopAllCoroutines();
        //find the item, move all the items in front to the back
    }

    
}
