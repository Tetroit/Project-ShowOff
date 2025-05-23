using Unity.VisualScripting;
using UnityEngine;

public class FloorBreak : MonoBehaviour
{
    [SerializeField] Collider _collider;
    public void Fall()
    {
        _collider.enabled = false;
    }
}
