using DG.Tweening;
using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    IEnumerator Interact();
    IEnumerator Deselect();
}

public interface IHoldable : IInteractable
{
    Transform Self { get; }
    Vector3 GetInitialPosition();
}

public readonly struct InputFacade
{
    readonly InputSystem_Actions _input;

    public InputFacade(InputSystem_Actions input)
    {
        _input = input;
    }

    public InputSystem_Actions.UIActions UI => _input.UI;
    public InputSystem_Actions.PlayerActions Player => _input.Player;
    
}

public class InteractionManager : MonoBehaviour
{
    [SerializeField] float _interactionRange;
    [SerializeField] float _interactionRadius;
    [SerializeField] LayerMask _interactionMask;
    [SerializeField] HoldManager _holdManager;
    
    Coroutine _interactAnimation;

    InputSystem_Actions _input;

    //Used to save performance by not getting component every frame
    IInteractable _lastInteractable;
    GameObject _lastInteractableGO;

    IInteractable _currentInteractingItem;

    void Awake()
    {
        _input = new();
        
        _input.Player.Interact.started += OnInteract;
        _input.UI.Cancel.started += OnDissmised;
    }
    
    void Start()
    {
        _holdManager.Init(new InputFacade(_input));
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (_lastInteractable == null || _interactAnimation != null) return;
        _currentInteractingItem = _lastInteractable;

        switch (_lastInteractable)
        {
            case IHoldable holdable:
                _interactAnimation = this.RunCoroutineWithCallback(_holdManager.OnInteract(holdable), () => _interactAnimation = null);
                break;
            default:
                break;
        }
    }


    void OnDissmised(InputAction.CallbackContext context)
    {
        if (_currentInteractingItem == null || _interactAnimation != null) return;
        
        _interactAnimation = this.RunCoroutineWithCallback(_holdManager.OnDismiss(), () => _interactAnimation = null);

        _currentInteractingItem = null;

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

            _lastInteractable = null;
            _lastInteractableGO = null;
            return;
        }


        if (hit.transform.gameObject == _lastInteractableGO)
        {
            _holdManager.OnItemHover(_lastInteractable as IHoldable, new HoverData(hit));
            return;
        }

        _lastInteractable = hit.transform.GetComponentInChildren<IInteractable>();
        _lastInteractableGO = hit.transform.gameObject;
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
