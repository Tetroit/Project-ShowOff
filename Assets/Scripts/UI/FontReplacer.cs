using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class FontReplacer : MonoBehaviour
{

    [SerializeField] TMP_FontAsset _fontAsset;
    [SerializeField] bool _execute;
    [SerializeField] List<GameObject> _prefabs;
    void Update()
    {
        if (_execute)
        {
            _execute = false;
            if (_fontAsset == null) return;

            var textObjects = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var textObject in textObjects)
            {
                textObject.font = _fontAsset;
            }

            foreach (var prefab in _prefabs)
            {
                if (prefab == null) continue;
                var prefabTexts = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var text in prefabTexts)
                {
                    text.font = _fontAsset;
                }
#if UNITY_EDITOR

                EditorUtility.SetDirty(prefab);
#endif
            }

#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif
        }
    }
}