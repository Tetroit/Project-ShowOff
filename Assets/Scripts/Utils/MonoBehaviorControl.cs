using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class MonoBehaviorControl : MonoBehaviour
{
    public List<MonoBehaviour> Collection;
    [SerializeField] string _typeToSearch;
    [ContextMenu("PopulateWithType")]
    public void PopulateWithType()
    {
        Type t = Type.GetType(_typeToSearch);
        if(t == null )
        {
            Debug.LogError("Invalid type");
            return;
        }

        Collection = GetComponentsInChildren(t).Select(c => c as MonoBehaviour).ToList();
#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(this, "added objects");
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    [ContextMenu("EnableAll")]
    public void EnableAll()
    {
        foreach (var item in Collection)
        {
            item.enabled = true;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(item, "enabled");
            UnityEditor.EditorUtility.SetDirty(item);
#endif
        }

    }
    [ContextMenu("DisableAll")]
    public void DisableAll()
    {
        foreach (var item in Collection)
        {
            item.enabled = false;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(item, "disabled");
            UnityEditor.EditorUtility.SetDirty(item);
#endif
        }
    }
}
