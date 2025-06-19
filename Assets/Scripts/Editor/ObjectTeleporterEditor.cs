using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectTeleporter))]
public class ObjectTeleporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ObjectTeleporter mono = target as ObjectTeleporter;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Target"));
        foreach (Transform point in mono.transform)
        {
            if(GUILayout.Button(new GUIContent(point.gameObject.name)))
            {
                mono.TeleportToPoint(point);

                if (SceneView.lastActiveSceneView != null)
                {
                    SceneView.lastActiveSceneView.LookAt(
                        point.position,
                        point.rotation,
                        1f // optional zoom distance
                    );
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
