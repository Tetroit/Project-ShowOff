using System;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerInputHandler// : MonoBehaviour where MonoBehaviour : succs ass
{
    public static PlayerInputHandler Instance;
    InputActionAsset actions => InputSystem.actions;
    string playerMapName => "Player";

    InputActionMap map;

    InputAction moveAction;
    InputAction viewAction;
    InputAction jumpAction;
    InputAction crouchAction;
    InputAction sprintAction;

    public static PlayerInputHandler Create()
    {
        if (Instance == null)
        {
            Instance = new();
            Instance.LazyInitialize();
        }
        return Instance;

    }
    public static bool IsInitialized => Instance != null;
    private PlayerInputHandler() { }
    public void LazyInitialize()
    {
        if (actions == null)
        {
            Debug.LogError("No input asset attached");
            return;
        }
        map = actions.FindActionMap(playerMapName);
        AssertActionMap(map);

        moveAction = map.FindAction("Move");
        AssertAction(moveAction);

        viewAction = map.FindAction("Look");
        AssertAction(viewAction);

        jumpAction = map.FindAction("Jump");
        AssertAction(jumpAction);

        sprintAction = map.FindAction("Sprint");
        AssertAction(sprintAction);

        crouchAction = map.FindAction("Crouch");
        AssertAction(crouchAction);

    }

    void AssertAction(InputAction action)
    {
        if (action == null)
            Debug.LogError($"Action {action.name} was not found");
    }
    void AssertActionMap(InputActionMap map)
    {
        if (map == null)
            Debug.LogError($"Action {map.name} was not found");
    }
    public Vector2 Move => moveAction.ReadValue<Vector2>();
    public Vector2 View => viewAction.ReadValue<Vector2>();
    public bool JumpPressedThisFrame => jumpAction.WasPressedThisFrame();
    public bool CrouchPressedThisFrame => crouchAction.WasPressedThisFrame();
    public bool SprintPressedThisFrame => sprintAction.WasPressedThisFrame();
    public bool JumpPressed => jumpAction.IsPressed();
    public bool CrouchPressed => crouchAction.IsPressed();
    public bool SprintPressed => sprintAction.IsPressed();
}
