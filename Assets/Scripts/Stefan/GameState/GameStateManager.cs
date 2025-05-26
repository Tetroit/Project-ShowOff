using amogus;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    UI,
    Play,
    Pause,
    Cutscene
}

public class GameStateManager : MonoBehaviour
{
    [field: SerializeField] public PlayerFSM PlayerController { get; private set; }
    [field: SerializeField] public InventoryController InventoryController { get; private set; }
    [field: SerializeField] public InteractionManager InteractionManager { get; private set; }
    [field: SerializeField] public Window PauseWindow { get; private set; }
    [field: SerializeField] public GameObject HUD { get; private set; }


    [SerializeField] GameState _gameState;

    GameState _currentStateKey;
    State _currentState;
    Dictionary<GameState, State> _states;

    GameState _previousState;

    public GameState CurrentState => _currentStateKey;


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
            { GameState.Cutscene, new S_Cutscene() },
        };

        foreach (KeyValuePair<GameState, State> kv in _states)
            kv.Value.Init(this);
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
        _previousState = _gameState;
        _currentStateKey = _gameState;
        _currentState = _states[_gameState];    
        _currentState.Enter();
    }

    public void SwitchToPrevious()
    {
        SwitchState(_previousState);
    }

    public void SwitchState(GameState state)
    {
        _previousState = _gameState;

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

    public void SwitchToCutscene()
    {
        SwitchState(GameState.Pause);
    }
}
