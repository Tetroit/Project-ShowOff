using UnityEngine;

public abstract class InventoryItemView : MonoBehaviour
{

    public virtual void Select()
    {
        if (gameObject != null)
            gameObject.SetActive(true);

    }

    public virtual void Deselect()
    {
        if(gameObject != null)
            gameObject.SetActive(false);

    }

    public virtual void Interact(GameObject user) { }

    public virtual void AddInInventory(GameObject user) { }
}
