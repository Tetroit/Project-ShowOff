using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryView : MonoBehaviour
{
    enum SelectDirection
    {
        Left = -1, Right = 1
    }

    //public UnityEvent<InventoryItem> OnItemSelect;
    //public UnityEvent<InventoryItem> OnItemDeselect;
    [SerializeField] int _capacity;
    [SerializeField] InventoryItemView _itemSlotPrefab;
    [SerializeField] Transform _itemsContainer;
    [SerializeField] Inventory _logic;
    [SerializeField] GameObject _holder;

    readonly List<InventoryItemView> _items = new();
    int _curentItemIndex;

    InputSystem_Actions _input;
    void Awake()
    {
        if (_itemsContainer == null) _itemsContainer = transform;
        _input = new();
        _curentItemIndex = -1;
    }

    void Start()
    {
        UpdateUI(_logic.Items);
    }

    public void UpdateUI(IEnumerable<InventoryItem> items)
    {
        _items.Clear();
        int i = 0;
        foreach (InventoryItem item in items)
        {
            if (i++ >= _capacity) break;

            InventoryItemView instance = Instantiate(_itemSlotPrefab, _itemsContainer);
            instance.SetItem(item);
            _items.Add(instance);
        }
            
    }

    void OnEnable()
    {
        _input.Enable();
        _input.UI.ScrollWheel.performed += OnScroll;
        _input.Player.Interact.started += OnInteract;
    }

    void OnDisable()
    {
        _input.Disable();
        _input.UI.ScrollWheel.performed -= OnScroll;
        _input.Player.Interact.started -= OnInteract;

    }

    void OnInteract(InputAction.CallbackContext context)
    {
        _curentItemIndex = Mathf.Clamp(_curentItemIndex, 0, _items.Count - 1);

        _items[_curentItemIndex].Item.Interact(_holder);
    }

    void OnScroll(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        if (value.y > 0) ChangeItemPosition(SelectDirection.Right);
        else if(value.y < 0) ChangeItemPosition(SelectDirection.Left);
    }
    

    void ChangeItemPosition(SelectDirection selectDirection)
    {
        ChangeItemPosition(_curentItemIndex + (int)selectDirection);
    }

    void ChangeItemPosition(int index)
    {
        index = Mathf.Clamp(index, 0, _items.Count - 1);
        if (index == _curentItemIndex) return;
        _curentItemIndex = Mathf.Clamp(_curentItemIndex, 0, _items.Count - 1);

        _items[_curentItemIndex].Deselect();
        _curentItemIndex = index;
        _items[_curentItemIndex].Select();
    }


}
