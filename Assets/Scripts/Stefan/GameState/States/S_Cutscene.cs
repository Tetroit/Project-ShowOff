using UnityEngine;

public class S_Cutscene : State
{
    bool _visibility;
    CursorLockMode _lockMode;
    bool _movement;
    bool _inventoryControl;
    bool _interactionsEnabled;
    int _selectedInventory;
    int _selectedItem;
    bool _hudActive;

    public override void Enter()
    {
        _visibility = Cursor.visible;
        _lockMode = Cursor.lockState;
        _movement = fsm.PlayerController.inAnimation;
        _inventoryControl = fsm.InventoryController.gameObject.activeSelf;
        _interactionsEnabled = fsm.InteractionManager.enabled;
        _selectedInventory = fsm.InventoryController.CurrentInventoryIndex;
        _selectedItem = fsm.InventoryController.GetCurrentInventory().CurentItemIndex;
        _hudActive = fsm.HUD.activeSelf;

        SetStates(
            cursoreVisibility: false, 
            lockMode: CursorLockMode.None,
            movement: false,
            inventoryControl: false,
            interactionsEnabled: false,
            inventorySelectIndex: fsm.InventoryController.CurrentInventoryIndex,
            itemSelectIndex: fsm.InventoryController.GetCurrentInventory().GetItemIndex("NoArm"),
            hudActive: false
            );
        //WindowManager.Instance.TrySwitchWindow(fsm.CutsceneWindow);
    }

    public override void Exit()
    {
        SetStates(
            cursoreVisibility: _visibility,
            lockMode: _lockMode,
            movement: _movement,
            inventoryControl: _inventoryControl,
            interactionsEnabled: _interactionsEnabled,
            inventorySelectIndex: _selectedInventory,
            itemSelectIndex: _selectedItem,
            hudActive: _hudActive
            );
        //WindowManager.Instance.SwitchToPrevious();

    }

    void SetStates
    (
        bool cursoreVisibility,
        CursorLockMode lockMode,
        bool movement,
        bool inventoryControl,
        bool interactionsEnabled,
        int inventorySelectIndex,
        int itemSelectIndex,
        bool hudActive
    )
    {
        Cursor.visible = cursoreVisibility;
        Cursor.lockState = lockMode;
        if (fsm.PlayerController != null) 
        if (movement)
            fsm.PlayerController.EnableControls();
        else
            fsm.PlayerController.DisableControls();
        if(fsm.InventoryController != null)
            fsm.InventoryController.gameObject.SetActive(inventoryControl);

        if(fsm.InteractionManager != null)
            fsm.InteractionManager.enabled = interactionsEnabled;

        if(fsm.InventoryController != null)
            fsm.InventoryController.SetInventory(inventorySelectIndex);
        if(fsm.InventoryController != null)
            fsm.InventoryController.GetCurrentInventory().ChangeItemPosition(itemSelectIndex);
        if(fsm.HUD != null)
            fsm.HUD.SetActive(hudActive);

    }
}
