using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class PhysicsTransform : MonoBehaviour
{
    [SerializeField] Transform _parent;
    [SerializeField] Vector3 _positionOffset;
    [SerializeField] Quaternion _rotationOffset;
    Rigidbody _body;
    Vector3 _worldPositionOffset;
    Quaternion _worldRotationOffset;



    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _worldPositionOffset = transform.TransformPoint(_positionOffset);
        _worldRotationOffset = transform.parent.rotation * _rotationOffset;
        transform.position = _parent.position;
    }

    void FixedUpdate()
    {
        if (_body == null) return;


        _body.MoveRotation(_parent.rotation );
        _body.MovePosition(_parent.position +_positionOffset);
    }
}
