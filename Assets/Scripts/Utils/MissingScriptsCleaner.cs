#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class MissingScriptsCleaner : MonoBehaviour
{
    [ContextMenu("Clean Missing MonoBehaviours In Children")]
    void CleanMissingScriptsInChildren()
    {
        int count = 0;
        var gameObjects = GetComponentsInChildren<Transform>(true);

        foreach (var t in gameObjects)
        {
            GameObject go = t.gameObject;
            int before = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
            if (before > 0)
            {
                Undo.RegisterCompleteObjectUndo(go, "Remove Missing Scripts");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                Debug.Log("Removed from " + go);
                count += before;
            }
            EditorUtility.SetDirty(go);
        }

        Debug.Log($"Removed {count} missing script reference(s) from hierarchy under '{gameObject.name}'.");
    }
}
#endif
