using UnityEngine;

public class RotationEaser : MonoBehaviour
{
    public Transform cameraTransform;

    [Header("Sway Settings")]
    public float PositionSwayMultiplier = 0.01f;
    public float MaxSwayDistance = 0.1f;
    public float ReturnSpeed = 5f;

    Vector3 _initialLocalPosition;
    Quaternion _previousCameraRotation;
    Vector3 _swayOffset;

    void Start()
    {
        if (!cameraTransform) cameraTransform = Camera.main.transform;

        _initialLocalPosition = transform.localPosition;
        _previousCameraRotation = cameraTransform.rotation;
        _swayOffset = Vector3.zero;
    }

    void LateUpdate()
    {
        if(Time.deltaTime == 0 ) return;

        Quaternion currentRotation = cameraTransform.rotation;
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(_previousCameraRotation);

        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;
        
        Vector3 angularVelocity = axis * (angle * Mathf.Deg2Rad / Time.deltaTime);
        Vector3 localAngular = cameraTransform.InverseTransformDirection(angularVelocity);
        Vector3 targetOffset = new Vector3(-localAngular.y, localAngular.x, 0f) * PositionSwayMultiplier;

        targetOffset = Vector3.ClampMagnitude(targetOffset, MaxSwayDistance);
        _swayOffset = Vector3.Lerp(_swayOffset, targetOffset, Time.deltaTime * ReturnSpeed);

        transform.localPosition = _initialLocalPosition + _swayOffset;
        _previousCameraRotation = currentRotation;
    }
}
