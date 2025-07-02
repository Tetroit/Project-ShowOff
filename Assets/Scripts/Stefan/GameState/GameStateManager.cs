using amogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


public class GameStateManager : MonoBehaviour
{
    [field: SerializeField] public PlayerFSM PlayerController { get; private set; }
    [field: SerializeField] public InventoryController InventoryController { get; private set; }
    [field: SerializeField] public InteractionManager InteractionManager { get; private set; }
    [field: SerializeField] public Window PauseWindow { get; private set; }
    [field: SerializeField] public GameObject HUD { get; private set; }


    Type _currentStateKey;
    State _currentState;
    Dictionary<Type, State> _states;

    readonly Stack<Type> _previousStates = new();

    public Type CurrentState => _currentStateKey;

#if UNITY_EDITOR
    bool _dependenciesMissing;
    [SerializeField] List<string> _stest;
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
        _states = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(State).IsAssignableFrom(t))
            .ToDictionary(
                t => t,
                t => (State)Activator.CreateInstance(t)
            );

        foreach (KeyValuePair<Type, State> kv in _states)
            kv.Value.Init(this);
    }

#if UNITY_EDITOR
    void Update()
    {
        _stest = _previousStates.Select(s=>s.Name).ToList();
    }
#endif

    void Start()
    {
        _currentStateKey = typeof(S_Play);
        _previousStates.Push(_currentStateKey);
        _currentState = _states[_currentStateKey];
        _currentState.Enter();
    }

    public void SwitchToPrevious()
    {
        _previousStates.Pop();
        ApplyState(_previousStates.Peek());
    }
    
    public void SwitchState(Type t)
    {
        _previousStates.Push(t);
        ApplyState(t);
    }

    public void SwitchState<T>() where T : State
    {
        Type t = typeof(T);
        SwitchState(t);
    }
    
    public void SwitchState(string state)
    {
        Type t = Type.GetType(state);
        SwitchState(t);
    }

    public void ApplyState<T>() where T : State
    {
        ApplyState(typeof(T));
    }

    public void ApplyState(Type state)
    {
        _currentState.Exit();
        _currentStateKey = state;
        _currentState = _states[_currentStateKey];
        _currentState.Enter();
    }

    public void ApplyState(string state)
    {
        Type t = Type.GetType(state);
        ApplyState(t);
    }

    //for unityevents
    public void SwitchToUI()
    {
        SwitchState<S_UI>();
    }

    public void SwitchToGameOver()
    {
        WindowManager.Instance.TrySwitchWindow("GameOverUI");
        SwitchState<S_Pause>();
    }

    public void SwitchToPlay()
    {
        SwitchState<S_Play>();
    }

    public void SwitchToPause()
    {
        SwitchState<S_Pause>();
    }

    public void SwitchToCutscene()
    {
        SwitchState<S_Cutscene>();
    }

    private void OnDisable()
    {
        try
        {
            _currentState.Exit();
            _previousStates.Clear();
        }
        catch{ }
    }
}
