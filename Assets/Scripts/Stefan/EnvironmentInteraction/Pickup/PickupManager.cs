using System.Collections;
using UnityEngine;

//for now it only supports picking up keys, it should be extended to decide what pickup item goes into what inventory
public class PickupManager : InteractionBehavior<IPickupable>
{
    public InventoryController Controller;
    public InventoryView KeyInventory; 
    IPickupable _item;
    [SerializeField] bool _selectAfterPickup;

    public override void Init(InputFacade input)
    {

    }

    public override IEnumerator OnDismiss()
    {
        var item = (_item as ItemPickup);
        _item = null;

        if (!item.PlayAnim) yield break;

        yield return item.Deselect();
        Dismissed?.Invoke(item.gameObject, item);

        Destroy(item.gameObject);
        _item = null;
    }

    public override IEnumerator OnInteract(IPickupable interactable)
    {
        _item = interactable;
        var item = (_item as ItemPickup);
        if (item == null || !item.PlayAnim) yield break;

        Interacted?.Invoke(item.gameObject, _item);
        if(_selectAfterPickup)
        {
            Controller.SwitchToInventory("KeyInventory");
        }
        KeyInventory.AddItem(interactable.ItemData);
        KeyInventory.ChangeItemPosition(SelectDirection.Right);

        yield return interactable.Interact();
    }

    public void PickUpImmediate(ItemPickup item)
    {
        KeyInventory.AddItem(item.ItemData);
        KeyInventory.ChangeItemPosition(SelectDirection.Right);
        Destroy(item.gameObject);
    }
}
