using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [SerializeField] List<InventoryView> _inventories;

    [field: SerializeField] public UnityEvent<InventoryView> OnInventoryChanged { get; private set; }

    int _currentInventoryIndex;
    InputSystem_Actions _input;

    void Awake()
    {
        _input = new();
    }

    void OnEnable()
    {
        _input.Enable();
        _input.UI.ScrollWheel.performed += OnScroll;
        _input.Player.Interact.started += OnInteract;
        _input.UI.InventoryModes.started += OnInventoryChange;
    }

    void OnDisable()
    {
        _input.Disable();
        _input.UI.ScrollWheel.performed -= OnScroll;
        _input.Player.Interact.started -= OnInteract;
        _input.UI.InventoryModes.started -= OnInventoryChange;

    }

    void Start()
    {
        foreach (var i in _inventories)
            i.gameObject.SetActive(false);
        if (GetCurrentInventory().ItemCount == 0) _currentInventoryIndex++;
        WrapCurrentIndex();
        GetCurrentInventory().gameObject.SetActive(true);
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        GetCurrentInventory().InteractCurrent();
    }

    //UI
    //book in arm inventory -> can pause, can interact with items but it will close book by switching to noArm
    //pause -> goes on top of every UI element, prevents any interaction
    //hold item screen -> can pause, cannot change inventory

    //make the armInventoryView a window, remove the No arm item view and add another empty inventory

    void OnInventoryChange(InputAction.CallbackContext context)
    {
        var nextInventoryIndex = WrapIndexOnOverflow(_currentInventoryIndex + 1, _inventories.Count);
        bool canChange = WindowManager.Instance.CanSwitchToWindow(_inventories[nextInventoryIndex].GetComponent<Window>());
        
        if (!canChange) return;

        //monodirectional
        GetCurrentInventory().gameObject.SetActive(false);
        
        _currentInventoryIndex++;
        WrapCurrentIndex();
        if (GetCurrentInventory().ItemCount == 0) _currentInventoryIndex++;
        WrapCurrentIndex();

        var current = GetCurrentInventory();
        current.gameObject.SetActive(true);
        OnInventoryChanged?.Invoke(current);
    }

    void WrapCurrentIndex()
    {
        _currentInventoryIndex = WrapIndexOnOverflow(_currentInventoryIndex, _inventories.Count);
    }

    int WrapIndexOnOverflow(int index, int count)
    {
        if (index >= count) index = 0;
        return index;
    }

    void OnScroll(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        InventoryView inv = GetCurrentInventory();
        if (inv == null) return;

        if (value.y > 0) inv.ChangeItemPosition(SelectDirection.Right);
        else if (value.y < 0) inv.ChangeItemPosition(SelectDirection.Left);
    }

    InventoryView GetCurrentInventory()
    {
        return _inventories == null || _inventories.Count == 0 ? null : _inventories[_currentInventoryIndex];
    }
}
