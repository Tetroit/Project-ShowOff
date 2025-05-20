using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public static class UnityEventUtility
{
#if UNITY_EDITOR
    /// <summary>
    /// Checks if a persistent listener is already added, and adds it if not.
    /// </summary>
    /// <typeparam name="T">The type of UnityEvent (UnityEvent, UnityEvent<T>, etc.)</typeparam>
    /// <param name="unityEvent">The UnityEvent instance</param>
    /// <param name="target">The target object that has the method</param>
    /// <param name="methodName">The name of the method to call</param>
    /// <param name="callback">The delegate to add if it doesn't exist</param>
    public static void AddPersistentListenerIfMissing(UnityEventBase unityEvent, Object target, UnityAction callback)
    {
        string methodName = callback.Method.Name;
        UnityEvent typedEvent = (UnityEvent)unityEvent;
        
        if (!HasPersistentListener(typedEvent, target, methodName))
        {
            UnityEventTools.AddPersistentListener(typedEvent, callback);
            EditorUtility.SetDirty(target);
            Debug.Log($"Added persistent listener: {methodName}");
        }
        else
        {
            Debug.Log($"Listener already exists: {methodName}");
        }
    }

    /// <summary>
    /// Checks if a persistent listener exists on a UnityEventBase.
    /// </summary>
    public static bool HasPersistentListener(UnityEvent unityEvent, Object target, string methodName)
    {
        int count = unityEvent.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            if (unityEvent.GetPersistentTarget(i) == target &&
                unityEvent.GetPersistentMethodName(i) == methodName)
            {
                return true;
            }
        }
        return false;
    }
#endif
}