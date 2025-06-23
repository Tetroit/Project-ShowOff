using UnityEngine;

public class RotationEaser : MonoBehaviour
{
    public Transform cameraTransform;

    [Header("Sway Settings")]
    public float PositionSwayMultiplier = 0.01f;
    public float MaxSwayDistance = 0.1f;
    public float ReturnSpeed = 5f;

    [Header("Bobbing Settings")]
    public float BobbingAmplitude = 0.01f;
    public float BobbingFrequency = 6f;
    public float BobSpeedMultiplier = 1f;
    public float StartUpSpeed = 1f;

    Vector3 _initialLocalPosition;
    Quaternion _previousCameraRotation;
    Vector3 _swayOffset;

    Vector3 _previousCameraPosition;
    float _bobbingTimer = 0f;
    float _startUpTime01;

    void Start()
    {
        if (!cameraTransform) cameraTransform = Camera.main.transform;

        _initialLocalPosition = transform.localPosition;
        _previousCameraRotation = cameraTransform.rotation;
        _previousCameraPosition = cameraTransform.position;
        _swayOffset = Vector3.zero;
    }

    void LateUpdate()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime == 0) return;

        // ---- SWAY ----
        Quaternion currentRotation = cameraTransform.rotation;
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(_previousCameraRotation);

        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        Vector3 angularVelocity = axis * (angle * Mathf.Deg2Rad / deltaTime);
        Vector3 localAngular = cameraTransform.InverseTransformDirection(angularVelocity);
        Vector3 targetOffset = new Vector3(-localAngular.y, localAngular.x, 0f) * PositionSwayMultiplier;

        targetOffset = Vector3.ClampMagnitude(targetOffset, MaxSwayDistance);
        _swayOffset = Vector3.Lerp(_swayOffset, targetOffset, deltaTime * ReturnSpeed);

        // ---- BOBBING ----
        Vector3 currentCameraPosition = cameraTransform.position;
        Vector3 deltaPosition = currentCameraPosition - _previousCameraPosition;

        // Project delta to horizontal plane
        Vector2 horizontalDelta = new (deltaPosition.x, deltaPosition.z);
        float movementSpeed = horizontalDelta.magnitude / deltaTime;

        if (movementSpeed > 0.01f)
        {
            _bobbingTimer += deltaTime * BobbingFrequency * movementSpeed * BobSpeedMultiplier;
            _startUpTime01 += deltaTime * StartUpSpeed;
        }
        else
        {
            //_bobbingTimer = Mathf.Lerp(_bobbingTimer, 0f, deltaTime * 5f); // Smooth reset
            _startUpTime01 -= deltaTime * StartUpSpeed;

        }
        _startUpTime01 = Mathf.Clamp01(_startUpTime01);
        float bobOffsetY = Mathf.Sin(_bobbingTimer) * BobbingAmplitude * _startUpTime01;

        // ---- APPLY ----
        transform.localPosition = _initialLocalPosition + _swayOffset + new Vector3(0f, bobOffsetY, 0f);

        // Update for next frame
        _previousCameraRotation = currentRotation;
        _previousCameraPosition = currentCameraPosition;
    }
}
