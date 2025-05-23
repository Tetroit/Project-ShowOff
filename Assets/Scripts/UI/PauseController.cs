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

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (_isPaused)
        {
            DisablePause();

        }
        else
        {
            _previousState = _gameStateManager.CurrentState;
            _gameStateManager.SwitchState(GameState.Pause);
            _isPaused = true;
        }
    }

    public void DisablePause()
    {
        _gameStateManager.SwitchState(_previousState);
        _isPaused = false;

    }
}
