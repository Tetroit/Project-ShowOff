using amogus;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    UI,
    Play,
    Pause
}

public class GameStateManager : MonoBehaviour
{
    [field: SerializeField] public PlayerFSM PlayerController { get; private set; }
    [field: SerializeField] public InventoryController InventoryController { get; private set; }
    [field: SerializeField] public InteractionManager InteractionManager { get; private set; }

    [SerializeField] GameState _gameState;

    GameState _currentStateKey;
    State _currentState;
    Dictionary<GameState, State> _states;

    [SerializeField] InputActionReference _playAction;
    [SerializeField] InputActionReference _uiAction;
    [SerializeField] InputActionReference _pauseAction;

#if UNITY_EDITOR
    bool _dependenciesMissing;
    void OnValidate()
    {
        if(_dependenciesMissing) return;

        if (PlayerController == null) PlayerController = FindFirstObjectByType<PlayerFSM>();
        if (InventoryController == null) InventoryController = FindFirstObjectByType<InventoryController>();
        if (InteractionManager == null) InteractionManager = FindFirstObjectByType<InteractionManager>();

        if(PlayerController == null)
        {
            _dependenciesMissing = true;
            Debug.LogWarning("Game State Manager doesn't have all dependecies");
        }
    }
#endif
    void Awake()
    {
        _states = new()
        {
            { GameState.UI, new S_UI() },
            { GameState.Play, new S_Play() },
            { GameState.Pause, new S_Pause() },
        };

        foreach (KeyValuePair<GameState, State> kv in _states)
            kv.Value.Init(this);


        if (_playAction == null) return;

        _playAction.action.started += ctx => SwitchState(GameState.Play);
        _playAction.action.Enable();
        _pauseAction.action.started += ctx => SwitchState(GameState.Pause);
        _pauseAction.action.Enable();
        _uiAction.action.started += ctx => SwitchState(GameState.UI);
        _uiAction.action.Enable();
    }

    void Update()
    {
        if(_currentStateKey != _gameState)
        {
            SwitchState(_gameState);
        }
    }

    void Start()
    {
        _currentStateKey = _gameState;
        _currentState = _states[_gameState];    
        _currentState.Enter();
    }

    public void SwitchState(GameState state)
    {
        _currentState.Exit();
        _gameState = state;
        _currentStateKey = state;
        _currentState = _states[state];
        _currentState.Enter();
    }
    //for unityevents
    public void SwitchToUI()
    {
        SwitchState(GameState.UI);
    }

    public void SwitchToPlay()
    {
        SwitchState(GameState.Play);
    }

    public void SwitchToPause()
    {
        SwitchState(GameState.Pause);
    }
}
