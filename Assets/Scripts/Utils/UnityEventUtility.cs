#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;


using UnityEditor;
using UnityEditor.Events;

public static class UnityEventUtility
{
    /// <summary>
    /// Checks if a persistent listener is already added, and adds it if not.
    /// </summary>
    /// <typeparam name="T">The type of UnityEvent (UnityEvent, UnityEvent<T>, etc.)</typeparam>
    /// <param name="unityEvent">The UnityEvent instance</param>
    /// <param name="target">The target object that has the method</param>
    /// <param name="methodName">The name of the method to call</param>
    /// <param name="callback">The delegate to add if it doesn't exist</param>
    public static void AddPersistentListenerIfMissing<T1, T2>(UnityEvent<T1, T2> unityEvent, Object target, UnityAction<T1, T2> callback)
    {
        string methodName = callback.Method.Name;

        if (!HasPersistentListener(unityEvent, target, methodName))
        {
            UnityEventTools.AddPersistentListener(unityEvent, callback);
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
    public static bool HasPersistentListener<T1, T2>(UnityEvent<T1, T2> unityEvent, Object target, string methodName)
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

    public static void CleanMissingUnityEvents(GameObject obj)
    {

        Component[] components = obj.GetComponents<Component>();

        foreach (Component comp in components)
        {
            if (comp == null) continue;

            CleanMissingUnityEvents(comp);
        }

    }

    public static void CleanMissingUnityEvents(Component comp)
    {
        FieldInfo[] fields = comp.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (var field in fields)
        {
            if (!typeof(UnityEventBase).IsAssignableFrom(field.FieldType)) continue;
            if (field.GetValue(comp) is not UnityEventBase) continue;

            SerializedObject serializedObject = new SerializedObject(comp);
            SerializedProperty prop = serializedObject.FindProperty(field.Name);

            if (prop == null || !prop.isArray) continue;

            for (int i = prop.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty listener = prop.GetArrayElementAtIndex(i);
                SerializedProperty target = listener.FindPropertyRelative("m_Target");

                if (target != null && target.objectReferenceValue == null)
                {
                    prop.DeleteArrayElementAtIndex(i);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
