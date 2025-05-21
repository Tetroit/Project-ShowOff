using System.Collections;
using UnityEngine;

public class PickupManager : InteractionBehavior<IPickupable>
{
    public InventoryView KeyInventory; 
    IPickupable _item;
    public override void Init(InputFacade input)
    {

    }

    public override IEnumerator OnDismiss()
    {
        yield return null;
        Destroy((_item as MonoBehaviour).gameObject);
        _item = null;
    }

    public override IEnumerator OnInteract(IPickupable interactable)
    {
        _item = interactable;
        KeyInventory.AddItem(interactable.ItemData);
        yield return null;
    }
}
