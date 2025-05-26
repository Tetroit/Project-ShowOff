using System.Collections.Generic;
using System.Linq;
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
    public GameObject User;
    [SerializeField] protected int capacity;
    [SerializeField] NavigationMode navigationMode; 
    [SerializeField] List<InventoryItemView> items = new();

    [field: SerializeField] public UnityEvent<InventoryItemView> ItemSelected { get; private set; }
    [field: SerializeField] public UnityEvent<InventoryItemView> ItemDeselected { get; private set; }
    int _curentItemIndex;
    
    public int ItemCount => items.Count;
    public int CurentItemIndex => _curentItemIndex;
    
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
        if(GetCurrentItem() != null)
            GetCurrentItem().Select();

    }

    void OnDisable()
    {
        if(GetCurrentItem() != null)
            GetCurrentItem().Deselect();
    }

    public void InteractCurrent()
    {
        
        _curentItemIndex = Mathf.Clamp(_curentItemIndex, 0, items.Count - 1);

        items[_curentItemIndex].Interact(User);
    }

    public void ChangeItemPosition(SelectDirection selectDirection)
    {
        ChangeItemPosition(_curentItemIndex + (int)selectDirection);
    }

    public int GetItemIndex(string name)
    {
        int index = 0;
        foreach (InventoryItemView item in items)
            if (item.gameObject.name != name) index++;
            else return index;

        return -1;
    }


    public void ChangeItemPosition(int index)
    {
        if(navigationMode == NavigationMode.Clamp)
            index = Mathf.Clamp(index, 0, items.Count - 1);
        else if(navigationMode == NavigationMode.Cycle)
        {
            if (index < 0) index = items.Count - 1;
            else if(index > items.Count - 1) index = 0;
        }

        if (index == _curentItemIndex) return;

        bool canChange = !items[index].TryGetComponent<Window>(out var window) || WindowManager.Instance.CanSwitchToWindow(window);

        if (!canChange) return;
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

    public void AddItem(InventoryItemView item)
    {
        var instance = Instantiate(item);
        items.Add(instance);
        instance.AddInInventory(User);
        ChangeItemPosition(SelectDirection.Right);
    }
    public void Clear()
    {
        items.Clear();
    }
}
