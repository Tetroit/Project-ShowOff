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
        if(!Physics.Raycast(
            _cam.ScreenPointToRay(Mouse.current.position.ReadValue()),
            out RaycastHit hit,
            1,
            1<< LayerMask.NameToLayer("Interactable"))
            )
        {
            return;
        }

        if (InteractionAnimation != null || CurrentInteractable == null) return;

        InteractionAnimation = this.RunCoroutineWithCallback(Rotate(), ()=> 
        {
            if (CurrentInteractable is ITextDisplayer textDisplayer)
                textDisplayer.Toggle();
            InteractionAnimation = null;
        });
    }

    IEnumerator Rotate()
    {
        Quaternion startRotation = baseRotation;
        Quaternion endRotation = baseRotation * Quaternion.Euler(0, 180, 0);
        _rotationState *= -1;

        float duration = _rotationTimeSeconds;
        yield return DOVirtual.Float(0f, 1f, duration, t =>
        {
            Quaternion currentRotation = Quaternion.Slerp(startRotation, endRotation, t);

            baseRotation = currentRotation;

        }).SetEase(_rotationEase).WaitForCompletion();
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
        holdable.Self.SetPositionAndRotation(_holdSpot.position, _holdSpot.rotation);
    }

    public override IEnumerator OnInteract(IHoldable interactable)
    {
        CurrentInteractable = interactable;
        Interacted?.Invoke(interactable.Self.gameObject ,interactable);
        baseRotation = _holdSpot.rotation;

        StartCoroutine(interactable.Interact());
        yield return StartCoroutine(GrabAnimation(interactable, _grabTimeSeconds));

        _itemHoldBehavior = StartCoroutine(OnItemHold(interactable));
    }

    public override IEnumerator OnDismiss()
    {
        CurrentInteractable.Self.DORotateQuaternion(CurrentInteractable.GetInitialRotation(), _grabTimeSeconds);
        StopCoroutine(_itemHoldBehavior);
        StartCoroutine(CurrentInteractable.Deselect());
        yield return CurrentInteractable.Self.DOMove(CurrentInteractable.GetInitialPosition(), _grabTimeSeconds).WaitForCompletion();
        Dismissed?.Invoke(CurrentInteractable.Self.gameObject, CurrentInteractable);

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

            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Camera cam = _cam;
            Vector3 objScreenPos = cam.WorldToScreenPoint(objTransform.position);
            Vector2 screenDir = (mouseScreenPos - new Vector2(objScreenPos.x, objScreenPos.y)).normalized;

            float maxAngle = 10f;
            Quaternion slightRotation = Quaternion.AngleAxis(maxAngle, new Vector3(-screenDir.y * _rotationState, screenDir.x, 0f));

            //relative to the base rotation
            Quaternion desiredRotation = baseRotation * slightRotation;

            float rotationSpeed = 5f;
            objTransform.SetPositionAndRotation
            (
                Vector3.Lerp(objTransform.position, _holdSpot.position, .3f), 
                Quaternion.Lerp(objTransform.rotation, desiredRotation, Time.deltaTime * rotationSpeed)
            );
            yield return null;

        }

    }

}
