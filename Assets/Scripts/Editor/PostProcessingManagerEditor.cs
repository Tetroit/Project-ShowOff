using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(PostProcessingManager))]
public class PostProcessingManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        // DrawDefaultInspector();

        var manager = (PostProcessingManager)target;

        if (GUILayout.Button ("Add Volume"))
        {
            manager.volumes.Add(null);
            manager.overrideWeights.Add(1f);
        }

        for (int i=0;  i<manager.volumes.Count; i++)
        {
            var volume = manager.volumes[i];

            GUILayout.BeginHorizontal();

            manager.volumes[i] = EditorGUILayout.ObjectField(manager.volumes[i], typeof(Volume), true) as Volume;
            if (GUILayout.Button("Remove Volume"))
            {
                manager.volumes.RemoveAt(i);
                break;
            }
            if (volume != manager.volumes[i])
            {
                volume = manager.volumes[i];
                EditorUtility.SetDirty(manager);

            }
            GUILayout.EndHorizontal();

            if (volume == null)
                continue;

            GUILayout.Label(volume.profile.name);

            float previousPriority = volume.priority;
            float previousWeight = volume.weight;

            volume.weight = EditorGUILayout.Slider("Weight: ", volume.weight, 0f, 1f);
            volume.priority = EditorGUILayout.FloatField("Priority: ", volume.priority);

            if (previousPriority != volume.priority || previousWeight != volume.weight)
            {
                EditorUtility.SetDirty(manager);
            }
        }

        //serializedObject.Update();
        //serializedObject.FindProperty("mainVolume").objectReferenceValue = manager.mainVolume;
        //serializedObject.ApplyModifiedProperties();
    }
}
