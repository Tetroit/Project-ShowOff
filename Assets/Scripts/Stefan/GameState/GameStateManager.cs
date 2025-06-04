using amogus;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    readonly Stack<GameState> _previousStates = new();

    public GameState CurrentState => _currentStateKey;

#if UNITY_EDITOR
    bool _dependenciesMissing;
    [SerializeField] List<GameState> _stest;
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

#if UNITY_EDITOR
    void Update()
    {

        if (_currentStateKey != _gameState)
        {
            SwitchState(_gameState);
        }

        _stest = _previousStates.ToList();
    }
#endif

    void Start()
    {
        _currentStateKey = _gameState;
        _previousStates.Push(_currentStateKey);
        _currentState = _states[_gameState];    
        _currentState.Enter();
    }

    public void SwitchToPrevious()
    {
        _previousStates.Pop();
        ApplyState(_previousStates.Peek());
    }

    public void SwitchState(GameState state)
    {
        _previousStates.Push(state);
        ApplyState(state);
    }

    public void ApplyState(GameState state)
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

    public void SwitchToCutscene()
    {
        SwitchState(GameState.Pause);
    }
}
