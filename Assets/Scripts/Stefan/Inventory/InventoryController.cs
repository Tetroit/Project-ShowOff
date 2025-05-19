using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [SerializeField] List<InventoryView> _inventories;

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
    }

    void OnDisable()
    {
        _input.Disable();
        _input.UI.ScrollWheel.performed -= OnScroll;
        _input.Player.Interact.started -= OnInteract;

    }

    void OnInteract(InputAction.CallbackContext context)
    {
        GetCurrentInventory().InteractCurrent();
    }

    void OnInventoryChange(InputAction.CallbackContext context)
    {
        //monodirectional
        _currentInventoryIndex++;
        if(_currentInventoryIndex >=  _inventories.Count)
            _currentInventoryIndex = 0;

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
