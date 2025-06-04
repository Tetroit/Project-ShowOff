using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IPickupable
{
    [SerializeField] InventoryItemView _item;

    public InventoryItemView ItemData => _item;

    public IEnumerator Deselect()
    {
        yield return null;
    }

    public IEnumerator Interact()
    {
        yield return null;
    }
}
