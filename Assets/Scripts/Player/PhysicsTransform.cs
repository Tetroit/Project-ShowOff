using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class PhysicsTransform : MonoBehaviour
{
    [SerializeField] Transform _parent;
    Rigidbody _body;



    void Start()
    {
        _body = GetComponent<Rigidbody>();
        transform.position = _parent.position;
    }

    void FixedUpdate()
    {
        if (_body == null) return;


        _body.MoveRotation(_parent.rotation);
        _body.MovePosition(_parent.position);
    }
}
