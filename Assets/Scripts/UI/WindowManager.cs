using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [Header("Leave empty to auto populate")]
    [SerializeField] Window[] _windows;

    [SerializeField] Window _currentActiveWindow;
    [SerializeField] int _historyCapacity = 10;
    [Header("Enabled for easy debugging")]
    [SerializeField] List<Window> _history = new();

    public static WindowManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if(_windows == null || _windows.Length == 0)
        {
            _windows = FindObjectsByType<Window>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }
    }

    public void TrySwitchWindow(string name)
    {
        Window w = _windows[_windows.IndexMatch(w => w.gameObject.name == name)];
        TrySwitchWindow(w);
    }

    public bool TrySwitchWindow(Window w)
    {
        if(_currentActiveWindow == null)
        {
            _currentActiveWindow = w;
            _currentActiveWindow.gameObject.SetActive(true);
            _history.Add(_currentActiveWindow);
            return true;
        }

        if (_currentActiveWindow.Priority < w.Priority)
        {
            _currentActiveWindow.gameObject.SetActive(false);
            _currentActiveWindow = w;
            _currentActiveWindow.gameObject.SetActive(true);

            _history.Add(_currentActiveWindow);
            if(_historyCapacity < _history.Count) _history.RemoveAt(0);
        }
        return false;
    }

    public void SwitchWindow(Window w)
    {
        if (_currentActiveWindow == null)
        {
            _currentActiveWindow = w;
            _currentActiveWindow.gameObject.SetActive(true);
            _history.Add(_currentActiveWindow);
            return;
        }

        if (_currentActiveWindow.Priority < w.Priority)
        {
            _currentActiveWindow.gameObject.SetActive(false);
            _currentActiveWindow = w;
            _currentActiveWindow.gameObject.SetActive(true);

            _history.Add(_currentActiveWindow);
            if (_historyCapacity < _history.Count) _history.RemoveAt(0);
        }
    }

    public void CloseCurrentWindow()
    {
        if(_currentActiveWindow != null)
        _currentActiveWindow.gameObject.SetActive(false);
        _currentActiveWindow = null;
        _history.RemoveAt(_history.Count - 1);
    }

    public bool CanSwitchToWindow(Window w)
    {
        return _currentActiveWindow == null || _currentActiveWindow.Priority <= w.Priority;
    }

    public void SwitchToPrevious()
    {
        if(_history.Count  == 0) return;

        if (_history.Count == 1)
        {
            _history.RemoveAt(0);
            if(_currentActiveWindow != null)
            _currentActiveWindow.gameObject.SetActive(false);
            _currentActiveWindow = null;
            return;
        }

        int lastI = _history.Count - 1;
        _history.RemoveAt(lastI--);

        Window lastW = _history[lastI];
        _currentActiveWindow.gameObject.SetActive(false);
        _currentActiveWindow = lastW;
        _currentActiveWindow.gameObject.SetActive(true);
    }
}
