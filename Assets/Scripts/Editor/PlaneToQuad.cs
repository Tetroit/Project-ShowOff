using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;

public static class PlaneToQuad
{
    [Shortcut("Tools/Convert Plane to Quad", KeyCode.G, ShortcutModifiers.Action)]
    public static void ConvertSelectedPlanes()
    {

        foreach (GameObject obj in Selection.gameObjects)
        {
            MeshFilter mf = obj.GetComponent<MeshFilter>();
            MeshCollider mc = obj.GetComponent<MeshCollider>();
            Transform t = obj.transform;

            if (mf == null || mf.sharedMesh == null)
                continue;

            Mesh planeMesh = mf.sharedMesh;

            if (planeMesh.vertexCount != 121 || planeMesh.triangles.Length != 600)
            {
                Debug.LogWarning($"Skipped '{obj.name}': not a Unity built-in Plane mesh.");
                continue;
            }

            Mesh quad = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            if (quad == null)
            {
                Debug.LogError("Couldn't load Unity built-in Quad mesh.");
                return;
            }

            // Store world transform
            Matrix4x4 oldWorld = t.localToWorldMatrix;

            // Replace mesh
            Undo.RecordObject(mf, "Replace Mesh");
            mf.sharedMesh = quad;

            if (mc != null)
            {
                Undo.RecordObject(mc, "Replace Collider Mesh");
                mc.sharedMesh = quad;
            }

            // Plane is 10x10 in XZ. Quad is 1x1 in XY.
            // So we apply a -90 X rotation and a 10x scale in X and Y.
            Matrix4x4 planeToQuad = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-90f, 0, 0), new Vector3(0.1f, 0.1f, 0.1f));

            // Calculate the new world transform that would produce the same world result with a quad
            Matrix4x4 newWorld = oldWorld * planeToQuad.inverse;

            // Decompose newWorld to position/rotation/scale
            Undo.RecordObject(t, "Update Transform");

            // Set position
            
            // Set rotation
            t.SetPositionAndRotation(
                newWorld.GetColumn(3), 
                Quaternion.LookRotation(
                    newWorld.GetColumn(2), // forward
                    newWorld.GetColumn(1)  // up
                )
            );

            // Compute local scale manually
            Vector3 newScale = new(
                newWorld.GetColumn(0).magnitude,
                newWorld.GetColumn(1).magnitude,
                newWorld.GetColumn(2).magnitude
            );
            t.localScale = newScale;
            EditorUtility.SetDirty( t );
            Debug.Log($"Converted '{obj.name}' to Quad while preserving visual appearance.");
        }
    }
}
