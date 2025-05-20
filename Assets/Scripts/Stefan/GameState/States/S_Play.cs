using UnityEngine;

public class S_Play : State
{
    //cursor is inactive, controller is active, game is normal speed
    bool _visibility;
    CursorLockMode _lockMode;
    bool _movement;

    public override void Enter()
    {
        _visibility = Cursor.visible;
        _lockMode = Cursor.lockState;
        _movement = fsm.PlayerController.inAnimation;

        SetStates(false || _movement, CursorLockMode.Locked, true);
    }

    public override void Exit()
    {
        SetStates(_visibility, _lockMode, _movement);
    }

    void SetStates(bool cursoreVisibility, CursorLockMode lockMode, bool movement)
    {
        Cursor.visible = cursoreVisibility;
        Cursor.lockState = lockMode;
        if (movement)
            fsm.PlayerController.EnableControls();
        else
            fsm.PlayerController.DisableControls();
    }
}
