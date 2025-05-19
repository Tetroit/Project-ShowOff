using UnityEngine;

public abstract class InventoryItemView : MonoBehaviour
{

    public virtual void Select()
    {
        gameObject.SetActive(true);

    }

    public virtual void Deselect()
    {
        gameObject.SetActive(false);

    }

    public virtual void Interact(GameObject user) { }
}
