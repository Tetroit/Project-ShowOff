using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.Experimental.GraphView.Port;
using UnityEngine;

public class PageInventoryView : InventoryView
{
    [SerializeField] InventoryItemView _itemSlotPrefab;
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

        }
    }

    void OnSelect(InventoryItemView item)
    {
        StopAllCoroutines();
        //find the item, move all the items in front to the back
    }


}
