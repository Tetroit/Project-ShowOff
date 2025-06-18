using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [field: SerializeField] public UnityEvent<GameObject, IInteractable> OnHover{get; protected set;}
    [field: SerializeField] public UnityEvent<GameObject, IInteractable> OnHoverStart{get;protected set;}
    [field: SerializeField] public UnityEvent<GameObject, IInteractable> OnHoverEnd { get; protected set; }

    [SerializeField] float _interactionRange;
    [SerializeField] float _interactionRadius;
    [SerializeField] LayerMask _interactionMask;
    //instead of having 
    [SerializeField] HoldManager _holdManager;
    [SerializeField] PickupManager _pickupManager;
    
    Coroutine _interactAnimation;

    InputSystem_Actions _input;

    //Used to save performance by not getting component every frame
    IInteractable _lastInteractable;
    GameObject _lastInteractableGO;
    RaycastHit _lastRaycastHit;

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

    public void Dissmised()
    {
        if (_currentInteractingItem == null || _interactAnimation != null) return;
        
        _interactAnimation = this.RunCoroutineWithCallback(_holdManager.OnDismiss(), () => _interactAnimation = null);

        _currentInteractingItem = null;
    }

    void HoverStart()
    {
        _lastInteractable = _lastRaycastHit.transform.GetComponentInChildren<IInteractable>();
        _lastInteractableGO = _lastRaycastHit.transform.gameObject;
        _holdManager.OnItemHoverStart(_lastInteractable as IHoldable, new HoverData(_lastRaycastHit));
        OnHoverStart?.Invoke(_lastInteractableGO, _lastInteractable);
    }

    void HoverEnd()
    {
        _holdManager.OnItemHoverEnd(_lastInteractable as IHoldable);
        OnHoverEnd?.Invoke(_lastInteractableGO, _lastInteractable);
        _lastInteractable = null;
        _lastInteractableGO = null;
    }

    void HoverStay()
    {
        _holdManager.OnItemHover(_lastInteractable as IHoldable, new HoverData(_lastRaycastHit));
        OnHover?.Invoke(_lastInteractableGO, _lastInteractable);
    }

    void FixedUpdate()
    {
        bool isInteractable = _currentInteractingItem == null && _interactAnimation == null && Physics.SphereCast
        (
            transform.position, 
            _interactionRadius,
            transform.forward,
            out _lastRaycastHit,
            _interactionRange,
            _interactionMask
        );

        if (!isInteractable)
        {
            if (_lastInteractable == null) return;
            HoverEnd();
            return;
        }

        if (_lastRaycastHit.transform.gameObject == _lastInteractableGO)
        {
            HoverStay();
            return;
        }

        HoverStart();

    }

    void OnEnable()
    {
        _input.Enable();
    }

    void OnDisable()
    {
        _input.Disable();
        if (_lastInteractable == null) return;

        _holdManager.OnItemHoverEnd(_lastInteractable as IHoldable);
        OnHoverEnd?.Invoke(_lastInteractableGO, _lastInteractable);

        _lastInteractable = null;
        _lastInteractableGO = null;
    }
}
