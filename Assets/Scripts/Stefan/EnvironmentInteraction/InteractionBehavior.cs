using System.Collections;
using UnityEngine;
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
    public T CurrentInteractable { get; protected set; }
    public Coroutine InteractionAnimation { get; set; }
    public abstract void Init(InputFacade input);

    public abstract IEnumerator OnInteract(T interactable);
    public abstract IEnumerator OnDismiss();

    public virtual void OnItemHover(T interactable, HoverData hitInfo) { }
    public virtual void OnItemHoverStart(T interactable, HoverData hitInfo) { }
    public virtual void OnItemHoverEnd(T interactable, HoverData hitInfo) { }
}
