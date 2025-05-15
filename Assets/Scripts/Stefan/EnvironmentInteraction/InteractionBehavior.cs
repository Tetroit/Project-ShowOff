using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public readonly struct HoverData
{
    public readonly RaycastHit HitInfo;

    public HoverData(RaycastHit hitInfo)
    {
        HitInfo = hitInfo;
    }
}
public abstract class InteractionBehavior<T> : MonoBehaviour where T : IInteractable
{
    [field: SerializeField] public UnityEvent<GameObject, T> Interacted { get; protected set; }
    [field: SerializeField] public UnityEvent<GameObject, T> Dismissed { get; protected set; }
    public T CurrentInteractable { get; protected set; }
    public Coroutine InteractionAnimation { get; set; }
    public abstract void Init(InputFacade input);

    public abstract IEnumerator OnInteract(T interactable);
    public abstract IEnumerator OnDismiss();

    public virtual void OnItemHover(T interactable, HoverData hitInfo) { }
    public virtual void OnItemHoverStart(T interactable, HoverData hitInfo) { }
    public virtual void OnItemHoverEnd(T interactable) { }
}
