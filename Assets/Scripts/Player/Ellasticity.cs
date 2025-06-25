using DG.Tweening;
using FMOD;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Elasticity : MonoBehaviour
{
    [Header("Elastic Settings")]
    [SerializeField] Vector3 _targetLocalPosition = Vector3.zero;
    [SerializeField] float _threshold = 0.01f;
    [SerializeField] float _elasticity = 10f;
    [SerializeField] float _damping = 1f;
    [SerializeField] float _force = 1f;

    Vector3 _previousPos;
    Rigidbody _body;

    void Start()
    {
        _body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (_body == null) return;

        
        Vector3 currentPosition = transform.localPosition;
        Vector3 toTarget = _targetLocalPosition - currentPosition;

        if (toTarget.sqrMagnitude > _threshold * _threshold)
        {
            _body.linearDamping = 0;
            // Apply spring force based on displacement and elasticity
            Vector3 force = toTarget * _force;
            _body.AddForce(force, ForceMode.Acceleration);
        }
        else
        {
            _body.linearDamping = _damping;
            transform.localPosition = Vector3.Lerp(_targetLocalPosition, currentPosition, _elasticity * Time.deltaTime);
        }
        _previousPos = transform.position;
    }

}

