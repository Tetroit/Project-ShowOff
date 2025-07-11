﻿using UnityEngine;

public class S_Pause : State
{
    //cursor is active, controller is inactive, game is 0 speed
    //should not allow controlling inventories
    bool _visibility;
    CursorLockMode _lockMode;
    bool _movement;
    float _gameTime;
    bool _inventoryControl;
    bool _interactionsEnabled;

    public override void Enter()
    {

        _visibility = Cursor.visible;
        _lockMode = Cursor.lockState;
        _movement = fsm.PlayerController.inAnimation;
        _gameTime = Time.timeScale;
        _inventoryControl = fsm.InventoryController != null ? fsm.InventoryController.gameObject.activeSelf : false;
        _interactionsEnabled = fsm.InteractionManager != null ? fsm.InteractionManager.enabled : false;

        SetStates(true, CursorLockMode.None, false, 0.0f, false, false);
        WindowManager.Instance.TrySwitchWindow(fsm.PauseWindow);
        AudioManager.instance.PauseAllAudio();
    }

    public override void Exit()
    {
        SetStates(_visibility, _lockMode, _movement, _gameTime, _inventoryControl, _interactionsEnabled);
        WindowManager.Instance.SwitchToPrevious();
        AudioManager.instance.UnPauseAllAudio();

    }

    void SetStates(bool cursoreVisibility, CursorLockMode lockMode, bool movement, float time, bool inventoryControl, bool interactionsEnabled)
    {
        Cursor.visible = cursoreVisibility;
        Cursor.lockState = lockMode;
        if(fsm.PlayerController != null) 
        if (movement)
            fsm.PlayerController.EnableControls();
        else
            fsm.PlayerController.DisableControls();
        Time.timeScale = time;
        if(fsm.InventoryController != null) 
        fsm.InventoryController.enabled = inventoryControl;
        if(fsm.InteractionManager != null) 
        fsm.InteractionManager.enabled = interactionsEnabled;
    }
}