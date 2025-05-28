using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    [SerializeField] InputActionReference _pauseAction;
    [SerializeField] GameStateManager _gameStateManager;

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
            _gameStateManager.SwitchState(GameState.Pause);
            _isPaused = true;
        }
    }

    public void DisablePause()
    {
        _gameStateManager.SwitchToPrevious();
        _isPaused = false;

    }
}
