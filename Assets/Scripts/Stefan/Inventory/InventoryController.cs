using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [SerializeField] List<InventoryView> _inventories;
    [SerializeField] float _itemChangeCooldown = .2f;
    [SerializeField] List<GameObject> _relatedUI;
    List<bool> _relatedUIStates = new();
    
    float _lastScrollTime;
    [field: SerializeField] public UnityEvent<InventoryView> OnInventoryChanged { get; private set; }

    int _currentInventoryIndex;
    InputSystem_Actions _input;


    public int CurrentInventoryIndex => _currentInventoryIndex;

    public void SetInventory(int index)
    {
        GetCurrentInventory().gameObject.SetActive(false);
        _currentInventoryIndex = index;
        var current = GetCurrentInventory();
        current.gameObject.SetActive(true);
        OnInventoryChanged?.Invoke(current);
    }

    public int GetInventoryIndex(string inventoryName)
    {
        return _inventories.IndexMatch(i => i.gameObject.name == inventoryName);
    }

    public void SwitchToInventory(string inventoryName)
    {
        SetInventory(GetInventoryIndex(inventoryName));
    }

    void Awake()
    {
        _input = new();

        for (int i = 0; i < _relatedUI.Count; i++)
        {
            _relatedUIStates.Add(_relatedUI[i].activeSelf);
        }
    }

    void OnEnable()
    {
        for (int i = 0; i < _relatedUI.Count; i++)
        {
            _relatedUI[i].SetActive( _relatedUIStates[i]);
        }

        _input.Enable();
        _input.UI.ScrollWheel.performed += OnScroll;
        _input.Player.Interact.started += OnInteract;
        _input.UI.InventoryModes.started += OnInventoryChange;
    }

    void OnDisable()
    {
        for (int i = 0; i < _relatedUI.Count; i++)
        {
            _relatedUIStates[i] = _relatedUI[i].activeSelf;
            _relatedUI[i].SetActive(false);
        }

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


    void OnInventoryChange(InputAction.CallbackContext context)
    {
        var prevIndex = _currentInventoryIndex; 
        var nextInventoryIndex = WrapIndexOnOverflow(_currentInventoryIndex + 1, _inventories.Count);
        if (prevIndex == nextInventoryIndex) return;

        var window = _inventories[nextInventoryIndex].GetComponent<Window>();
        bool canChange = WindowManager.Instance.CanSwitchToWindow(window);
        
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
        float currentTime = Time.time;
        if(currentTime - _lastScrollTime < _itemChangeCooldown)
        {
            return;
        }
        _lastScrollTime = currentTime;

        Vector2 value = context.ReadValue<Vector2>();
        InventoryView inv = GetCurrentInventory();
        if (inv == null) return;

        if (value.y > 0) inv.ChangeItemPosition(SelectDirection.Right);
        else if (value.y < 0) inv.ChangeItemPosition(SelectDirection.Left);
    }

    public InventoryView GetCurrentInventory()
    {
        return _inventories == null || _inventories.Count == 0 ? null : _inventories[_currentInventoryIndex];
    }
}
