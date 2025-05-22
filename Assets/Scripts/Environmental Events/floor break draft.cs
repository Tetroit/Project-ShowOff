using Unity.VisualScripting;
using UnityEngine;

public class FloorBreak : MonoBehaviour
{
    [SerializeField] Collider floorCollider;
    public void Fall()
    {
        floorCollider.enabled = false;
    }
}
