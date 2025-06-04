using DG.Tweening;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class JadeHold : HoldManager
{
    [SerializeField] float _rotationSensitivity = 1;
    [SerializeField] float _grabTimeSeconds = .5f;
    [SerializeField] float _drag;
    [SerializeField] Transform _holdSpot;

    float _currentHoldRotation;
    float _currentForce;
    float _currentForceSign;
    bool _isClick;

    InputFacade _input;

    public override void Init(InputFacade input)
    {
        _input = input;
        _input.UI.Click.started += OnMouseClickStart;
        _input.UI.Click.canceled += OnMouseClickEnd;

        _input.Player.Look.performed += OnMouseMove;
    }

    void OnMouseClickStart(InputAction.CallbackContext context)
    {
        _isClick = true;
    }

    void OnMouseClickEnd(InputAction.CallbackContext context)
    {
        _isClick = false;
    }

    void OnMouseMove(InputAction.CallbackContext context)
    {
        if (!_isClick || InteractionAnimation != null || CurrentInteractable == null) return;

        Vector2 delta = context.ReadValue<Vector2>();
        _currentForce = delta.x * _rotationSensitivity * Time.deltaTime;
        _currentForceSign = math.sign(_currentForce);
        _currentHoldRotation += _currentForce;
        _currentForce *= _currentForceSign;

        //_currentHoldRotation = Mathf.Clamp01(_currentHoldRotation);
        Vector3 euler = _holdSpot.eulerAngles;
        _holdSpot.rotation = Quaternion.Euler(euler.x, _currentHoldRotation, euler.z);

    }

    void FixedUpdate()
    {
        if (CurrentInteractable != null)
            UpdateHoldInertia();
    }

    void UpdateHoldInertia()
    {
        Vector3 euler = _holdSpot.eulerAngles;
        _currentHoldRotation += math.max(0, _currentForce) * _currentForceSign;
        _holdSpot.rotation = Quaternion.Euler(euler.x, _currentHoldRotation, euler.z);
        _currentForce -= _drag * Time.deltaTime;
    }

    IEnumerator GrabAnimation(IHoldable holdable, float time)
    {
        float currentTime = 0;
        float t = 0;
        holdable.Self.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);

        while (t < 1)
        {
            currentTime += Time.deltaTime;
            t = currentTime / time;

            holdable.Self.SetPositionAndRotation
            (
                Vector3.Slerp(startPos, _holdSpot.position, t),
                Quaternion.Slerp(startRot, _holdSpot.rotation, t)
            );
            yield return null;
        }
        holdable.Self.parent = _holdSpot;
        holdable.Self.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public override IEnumerator OnInteract(IHoldable interactable)
    {
        CurrentInteractable = interactable;
        StartCoroutine(interactable.Interact());
        _currentHoldRotation = 0;
        yield return StartCoroutine(GrabAnimation(interactable, _grabTimeSeconds));
    }

    public override IEnumerator OnDismiss()
    {
        CurrentInteractable.Self.DORotateQuaternion(Quaternion.identity, _grabTimeSeconds);
        yield return CurrentInteractable.Self.DOMove(CurrentInteractable.GetInitialPosition(), _grabTimeSeconds).WaitForCompletion();
        CurrentInteractable = null;
    }
}
