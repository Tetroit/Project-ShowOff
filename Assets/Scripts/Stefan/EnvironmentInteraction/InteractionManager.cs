using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public interface IInteractable
{
//    bool CanInteract();

    IEnumerator Interact();
    IEnumerator Deselect();
}

public interface IHoldable : IInteractable
{
    Transform Self { get; }
    Vector3 GetInitialPosition();
    Quaternion GetInitialRotation();
}

public interface ITextDisplayer
{
    public void Activate();
    public void Deactivate();
    public void Toggle();
}

public interface IPickupable : IInteractable
{
    InventoryItemView ItemData { get; }
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
    [field: SerializeField] public UnityEvent<GameObject, IInteractable> OnHover{get; protected set;}
    [field: SerializeField] public UnityEvent<GameObject, IInteractable> OnHoverStart{get;protected set;}
    [field: SerializeField] public UnityEvent<GameObject, IInteractable> OnHoverEnd { get; protected set; }

    [SerializeField] float _interactionRange;
    [SerializeField] float _interactionRadius;
    [SerializeField] LayerMask _interactionMask;
    [SerializeField] HoldManager _holdManager;
    [SerializeField] PickupManager _pickupManager;
    
    Coroutine _interactAnimation;

    InputSystem_Actions _input;

    //Used to save performance by not getting component every frame
    IInteractable _lastInteractable;
    GameObject _lastInteractableGO;

    IInteractable _currentInteractingItem;

    void Awake()
    {
        _input = new();
        
        _input.Player.Interact.started += InteractInput;
        _input.UI.Cancel.started += (c)=> Dissmised();
    }

    void Start()
    {
        _holdManager.Init(new InputFacade(_input));
    }

    void InteractInput(InputAction.CallbackContext context)
    {
        if (_currentInteractingItem == null)
            Interact();
        else
            Dissmised();
    }

    void Interact()
    {
        if (_lastInteractable == null || _interactAnimation != null) return;
        _currentInteractingItem = _lastInteractable;

        switch (_lastInteractable)
        {
            case IHoldable holdable:
                _interactAnimation = this.RunCoroutineWithCallback(_holdManager.OnInteract(holdable), () => _interactAnimation = null);
                break;
            case IPickupable pickupable:
                _interactAnimation = this.RunCoroutineWithCallback(_pickupManager.OnInteract(pickupable), () => 
                {
                    _interactAnimation = null;

                    if (_currentInteractingItem == null) return;

                    _interactAnimation = this.RunCoroutineWithCallback(_pickupManager.OnDismiss(), () => _interactAnimation = null);

                    _currentInteractingItem = null;
                });
                break;
            default:
                break;
        }
    }


    void Dissmised()
    {
        if (_currentInteractingItem == null || _interactAnimation != null) return;
        
        _interactAnimation = this.RunCoroutineWithCallback(_holdManager.OnDismiss(), () => _interactAnimation = null);

        _currentInteractingItem = null;
    }
    
    void FixedUpdate()
    {
        RaycastHit hit = new();
        bool isInteractable = _currentInteractingItem == null && _interactAnimation == null && Physics.SphereCast
        (
            transform.position, 
            _interactionRadius,
            transform.forward,
            out hit,
            _interactionRange,
            _interactionMask
        );

        if (!isInteractable)
        {

            if (_lastInteractable == null) return;

            _holdManager.OnItemHoverEnd(_lastInteractable as IHoldable);
            OnHoverEnd?.Invoke(_lastInteractableGO, _lastInteractable);

            _lastInteractable = null;
            _lastInteractableGO = null;
            return;
        }

        if (hit.transform.gameObject == _lastInteractableGO)
        {
            _holdManager.OnItemHover(_lastInteractable as IHoldable, new HoverData(hit));
            OnHover?.Invoke(_lastInteractableGO, _lastInteractable);
            return;
        }
        

        _lastInteractable = hit.transform.GetComponentInChildren<IInteractable>();
        _lastInteractableGO = hit.transform.gameObject;

        _holdManager.OnItemHoverStart(_lastInteractable as IHoldable, new HoverData(hit));
        OnHoverStart?.Invoke(_lastInteractableGO, _lastInteractable);
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
