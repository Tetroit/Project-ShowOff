using Unity.VisualScripting;
using UnityEngine;

public class FloorBreak : MonoBehaviour
{
    [SerializeField] Collider collider;
    public void Fall()
    {
        collider.enabled = false;
    }
}
