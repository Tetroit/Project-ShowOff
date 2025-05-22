using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    [SerializeField] InputActionReference _pauseAction;
    [SerializeField] GameStateManager _gameStateManager;

    GameState _previousState;
    bool _isPaused;

    void Awake()
    {
        _pauseAction.action.started += TogglePause;
        _pauseAction.action.Enable();
    }

    void Start()
    {
        _isPaused = _gameStateManager.CurrentState == GameState.Pause;
    }

    void TogglePause(InputAction.CallbackContext context)
    {
        if (_isPaused)
        {
            _gameStateManager.SwitchState(_previousState);
        }
        else
        {
            _gameStateManager.SwitchState(GameState.Pause);
        }
    }
}
