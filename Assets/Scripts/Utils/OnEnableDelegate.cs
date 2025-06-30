using UnityEngine;
using UnityEngine.Events;

public class OnEnableDelegate : MonoBehaviour
{
    public UnityEvent OnEnabled;
    public UnityEvent OnDisabled;
    void OnEnable()
    {
        OnEnabled?.Invoke();    
    }

    void OnDisable()
    {
        OnDisabled?.Invoke();

    }
}
