using NUnit.Framework;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PositionRandomizer : MonoBehaviour
{
    [SerializeField] private BoxCollider _col;
    [SerializeField] private List<Transform> _objects;

    [ContextMenu("Randomize")]
    public void Randomize()
    {
        if (_col == null)
        {
            Debug.LogWarning("BoxCollider is not assigned.");
            return;
        }

        Bounds bounds = _col.bounds;

        foreach (Transform obj in _objects)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            obj.position = randomPosition;

            Vector3 eulerAngles = obj.eulerAngles;
            eulerAngles.y = Random.Range(0f, 360f);
            obj.eulerAngles = eulerAngles;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif

        }
    }


    [SerializeField] Vector2 _minMaxAnimationSpeeds = new Vector2(0.5f, 1.5f);
    private void OnEnable()
    {
        foreach (var arm in _objects)
        {
            Animator animator = arm.GetComponent<Animator>();
            animator.speed = Random.Range(0.5f, 1.5f); // Different speeds
        }
    }
}
