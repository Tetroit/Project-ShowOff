using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTeleporter : MonoBehaviour
{
    public GameObject Target;

    public void TeleportToPoint(Transform point)
    {
        Target.transform.SetPositionAndRotation(point.position, point.rotation);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(Target.transform);
#endif
    }
}
