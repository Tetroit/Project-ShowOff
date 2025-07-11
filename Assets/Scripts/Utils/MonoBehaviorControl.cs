using FMODUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonoBehaviorControl : MonoBehaviour
{
    [Header("Use Inspector Context Menu For Functions")]
    public List<MonoBehaviour> Collection;
    [SerializeField] string _typeToSearch;
    [SerializeField] private EventReference gearAmbience;
    [SerializeField] private EventReference gearConstant;
    [SerializeField] private GameObject targetGear;
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
        if(targetGear != null)
        {
            //AudioManager.instance.PlayOneShot(gearAmbience, transform.position);
            //AudioManager.instance.SetAmbienceArea(AmbienceArea.GEAR_AREA);
            AudioManager.instance.PlayOneShot(gearConstant, targetGear.transform.position);
        }
        else
        {
            Debug.LogWarning("Sound not playing because targetGear is not assigned!");
        }
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
