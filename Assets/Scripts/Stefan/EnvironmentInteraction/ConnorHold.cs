using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConnorHold : HoldManager
{
    [SerializeField] float _rotationTimeSeconds = 1;
    [SerializeField] Ease _rotationEase;
    [SerializeField] float _grabTimeSeconds = .5f;
    [SerializeField] Transform _holdSpot;
    InputFacade _input;
    Camera _cam;
    Coroutine _itemHoldBehavior;
    Quaternion baseRotation = Quaternion.identity;
    int _rotationState = 1;

    public override void Init(InputFacade input)
    {
        _input = input;
        _input.UI.Click.started += OnMouseClick;
        _cam = Camera.main;

    }

    void OnMouseClick(InputAction.CallbackContext context)
    {
        if (InteractionAnimation != null || CurrentInteractable == null) return;

        InteractionAnimation = this.RunCoroutineWithCallback(Rotate(), ()=> InteractionAnimation = null);
    }

    IEnumerator Rotate()
    {
        baseRotation = _holdSpot.rotation * Quaternion.Euler(0, 180, 0);
        _rotationState *= -1;
        yield return _holdSpot.DORotateQuaternion(baseRotation, _rotationTimeSeconds)
            .SetEase(_rotationEase)
            .WaitForCompletion();

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
        yield return StartCoroutine(GrabAnimation(interactable, _grabTimeSeconds));

        baseRotation = transform.rotation;
        _itemHoldBehavior = StartCoroutine(OnItemHold(interactable));
    }

    public override IEnumerator OnDismiss()
    {
        CurrentInteractable.Self.DORotateQuaternion(Quaternion.identity, _grabTimeSeconds);
        StopCoroutine(_itemHoldBehavior);
        yield return CurrentInteractable.Self.DOMove(CurrentInteractable.GetInitialPosition(), _grabTimeSeconds).WaitForCompletion();
        CurrentInteractable = null;
    }

    IEnumerator OnItemHold(IHoldable interactable)
    {
        while(interactable != null )
        {
            //pause while rotating or other animations
            if (InteractionAnimation != null)
            {
                yield return null;
                continue;
            }

            Transform objTransform = interactable.Self;

            // Get mouse position in screen space
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

            // Convert object position to screen space
            Camera cam = _cam;
            Vector3 objScreenPos = cam.WorldToScreenPoint(objTransform.position);

            // Direction in screen space (2D)
            Vector2 screenDir = (mouseScreenPos - new Vector2(objScreenPos.x, objScreenPos.y)).normalized;

            // Compute the axis and small angle based on screen direction
            float maxAngle = 10f;
            Quaternion slightRotation = Quaternion.AngleAxis(maxAngle, new Vector3(-screenDir.y * _rotationState, screenDir.x, 0f));

            // Apply the slight tilt **relative to the base rotation**
            Quaternion desiredRotation = baseRotation * slightRotation;

            // Smoothly interpolate toward the result
            float rotationSpeed = 5f;
            objTransform.rotation = Quaternion.Lerp(objTransform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
            yield return null;

        }

    }

}
