using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum SelectDirection
{
    Left = -1, Right = 1
}
public enum NavigationMode
{
    Clamp,
    Cycle
}

public class InventoryView : MonoBehaviour
{
    [SerializeField] protected int capacity;
    [SerializeField] GameObject _user;
    [SerializeField] NavigationMode navigationMode; 
    [SerializeField] List<InventoryItemView> items = new();

    [field: SerializeField] public UnityEvent<InventoryItemView> ItemSelected { get; private set; }
    [field: SerializeField] public UnityEvent<InventoryItemView> ItemDeselected { get; private set; }
    int _curentItemIndex;
    
    public int ItemCount => items.Count;
    
    void Awake()
    {
        OnAwake();
        
    }
    void Start()
    {
        OnStart(); 
    }

    void OnEnable()
    {
        GetCurrentItem()?.Select();

    }

    void OnDisable()
    {
        GetCurrentItem()?.Deselect();
    }

    //DON'T FORGET TO SET THE NOTE AS A PARENT WHILE HOLDING ITEM
    public void InteractCurrent()
    {
        _curentItemIndex = Mathf.Clamp(_curentItemIndex, 0, items.Count - 1);

        items[_curentItemIndex].Interact(_user);
    }

    public void ChangeItemPosition(SelectDirection selectDirection)
    {
        ChangeItemPosition(_curentItemIndex + (int)selectDirection);
    }

    void ChangeItemPosition(int index)
    {
        if(navigationMode == NavigationMode.Clamp)
            index = Mathf.Clamp(index, 0, items.Count - 1);
        else if(navigationMode == NavigationMode.Cycle)
        {
            if (index < 0) index = items.Count - 1;
            else if(index > items.Count - 1) index = 0;
        }

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
        if(items.Count == 0) return null;
        return items[_curentItemIndex];
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
}
