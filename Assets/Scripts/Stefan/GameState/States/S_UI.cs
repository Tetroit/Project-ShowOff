using UnityEngine;

public class S_UI : State
{
    bool _visibility;
    CursorLockMode _lockMode;
    bool _movement;

    public override void Enter()
    {
        _visibility = Cursor.visible;
        _lockMode = Cursor.lockState;
        _movement = fsm.PlayerController.inAnimation;

        SetStates(true, CursorLockMode.None, false);
    }

    public override void Exit()
    {
        SetStates(_visibility, _lockMode, _movement);
    }

    void SetStates(bool cursoreVisibility, CursorLockMode lockMode, bool movement)
    {
        Cursor.visible = cursoreVisibility;
        Cursor.lockState = lockMode;
        if(movement)
            fsm.PlayerController.EnableControls();
        else
            fsm.PlayerController.DisableControls();
    }
}
