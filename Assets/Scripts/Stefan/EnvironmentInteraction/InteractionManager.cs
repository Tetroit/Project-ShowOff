using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Deselect();
}

public interface IHoldable : IInteractable
{
    void Hold();
    Transform Self { get; }
}

public class InteractionManager : MonoBehaviour
{
    [SerializeField] float _interactionRange;
    [SerializeField] float _interactionRadius;
    [SerializeField] LayerMask _interactionMask;
    [SerializeField] Transform _holdSpot;
    [SerializeField] Transform _faceDirection;
    [SerializeField] Transform _backwardsDirection;

    [SerializeField] float _rotationSensitivity;

    float _currentHoldRotation;
    bool _isClick;
    Coroutine _holdAnimation;
    bool _isHolding;

    InputSystem_Actions _input;

    IInteractable _lastInteractable;
    GameObject _lastInteractableGO;

    void Awake()
    {
        _input = new();
        _input.UI.Click.started += OnMouseClickStart;
        _input.UI.Click.canceled += OnMouseClickEnd;

        _input.Player.Look.performed += OnMouseMove;
        _input.Player.Interact.started += OnInteract;

    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (_lastInteractable == null || _holdAnimation != null) return;
        Debug.Log("interacting");
        switch (_lastInteractable)
        {
            case IHoldable holdable:
                PickUp(holdable);
                break;
            default:
                break;
        }
    }

    void OnMouseClickStart(InputAction.CallbackContext context)
    {
        _isClick = true;
        Debug.Log("click");
    }

    void OnMouseClickEnd(InputAction.CallbackContext context)
    {
        _isClick = false;
    }

    void OnMouseMove(InputAction.CallbackContext context)
    {
        if (!_isClick || _lastInteractable == null || _holdAnimation != null || !_isHolding) return;
        
        Vector2 delta = context.ReadValue<Vector2>();
        _currentHoldRotation += delta.x * _rotationSensitivity * Time.deltaTime;

        _currentHoldRotation = Mathf.Clamp01(_currentHoldRotation);

        RotateHoldedItem();
    }

    

    void FixedUpdate()
    {
        bool isInteractable = Physics.SphereCast
        (
            transform.position, 
            _interactionRadius,
            transform.forward,
            out RaycastHit hit,
            _interactionRange,
            _interactionMask
        );

        if (!isInteractable)
        {
            if (_lastInteractable == null) return;
            Debug.Log("not see interactable");

            _lastInteractable = null;
            _lastInteractable = null;
            return;
        }
        if (hit.transform.gameObject == _lastInteractableGO) return;

        _lastInteractable = hit.transform.GetComponentInChildren<IInteractable>();
        _lastInteractableGO = hit.transform.gameObject;
        Debug.Log("see interactable: " + _lastInteractableGO);
    }

    public void PickUp(IHoldable holdable)
    {
        holdable.Hold();
        _isHolding = true;
        _currentHoldRotation = 0;
        _holdAnimation = StartCoroutine(GrabAnimation(holdable, 1));
    }

    IEnumerator GrabAnimation(IHoldable holdable,float time)
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
        _holdAnimation = null;
    }

    void RotateHoldedItem()
    {
        _holdSpot.forward = Vector3.Slerp(_faceDirection.forward, _backwardsDirection.forward, _currentHoldRotation);
    }

    void OnEnable()
    {
        _input.Enable();
    }

    void OnDisable()
    {
        _input.Disable();
    }
}
