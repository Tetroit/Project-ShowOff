using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum SelectDirection
{
    Left = -1, Right = 1
}

public abstract class InventoryView : MonoBehaviour
{
    [SerializeField] protected int capacity;
   
    [SerializeField] Inventory _logic;
    [SerializeField] GameObject _holder;

    protected readonly List<InventoryItemView> items = new();
    int _curentItemIndex;
    [field: SerializeField] public UnityEvent<InventoryItemView> ItemSelected { get; private set; }
    [field: SerializeField] public UnityEvent<InventoryItemView> ItemDeselected { get; private set; }


    void Awake()
    {
        _curentItemIndex = -1;
    }

    void Start()
    {
        UpdateUI(_logic.Items);
    }

    public abstract void UpdateUI(IEnumerable<InventoryItem> items);
    

//DON'T FORGET TO SET THE NOTE AS A PARENT WHILE HOLDING ITEM
    public void InteractCurrent()
    {
        _curentItemIndex = Mathf.Clamp(_curentItemIndex, 0, items.Count - 1);

        items[_curentItemIndex].Item.Interact(_holder);
    }

    public void ChangeItemPosition(SelectDirection selectDirection)
    {
        ChangeItemPosition(_curentItemIndex + (int)selectDirection);
    }

    void ChangeItemPosition(int index)
    {
        index = Mathf.Clamp(index, 0, items.Count - 1);
        if (index == _curentItemIndex) return;
        _curentItemIndex = Mathf.Clamp(_curentItemIndex, 0, items.Count - 1);

        InventoryItemView current = GetCurrentItem();
        current.Deselect();
        ItemDeselected?.Invoke(current);
        _curentItemIndex = index;
        current = GetCurrentItem();
        current.Select();
        ItemSelected?.Invoke(current);
    }

    protected InventoryItemView GetCurrentItem()
    {
        return items[_curentItemIndex];
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
}
