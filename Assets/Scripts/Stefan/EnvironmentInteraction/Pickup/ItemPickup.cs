using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour, IPickupable
{
    [SerializeField] InventoryItemView _item;
    [SerializeField] UnityEvent OnPickUp;

    public InventoryItemView ItemData => _item;

    public IEnumerator Deselect()
    {
        yield return null;
    }

    public IEnumerator Interact()
    {
        OnPickUp.Invoke();
        Debug.Log("Pickup");
        yield return null;
    }
}
