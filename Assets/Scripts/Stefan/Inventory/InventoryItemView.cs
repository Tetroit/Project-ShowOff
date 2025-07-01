using UnityEngine;
using UnityEngine.Events;

public abstract class InventoryItemView : MonoBehaviour
{
    public bool SeeTheItem = true;

    public UnityEvent OnSelect;
    public UnityEvent OnDeselect;
    public virtual void Select()
    {
        if (!SeeTheItem) return;

        if (gameObject != null)
            gameObject.SetActive(true);
        OnSelect?.Invoke();
    }

    public virtual void Deselect()
    {
        if (!SeeTheItem) return;

        if (gameObject != null)
            gameObject.SetActive(false);
        OnDeselect?.Invoke();

    }

    public virtual void Interact(GameObject user) { }

    public virtual void AddInInventory(GameObject user) { }
}
