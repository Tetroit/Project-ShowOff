﻿using System.Collections;
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
        yield return _item.Deselect();
        var item = (_item as MonoBehaviour);
        Dismissed?.Invoke(item.gameObject, _item);

        Destroy(item.gameObject);
        _item = null;
    }

    public override IEnumerator OnInteract(IPickupable interactable)
    {
        _item = interactable;
        Interacted?.Invoke((_item as MonoBehaviour).gameObject, _item);
        if(_selectAfterPickup)
        {
            Controller.SwitchToInventory("KeyInventory");
        }
        KeyInventory.AddItem(interactable.ItemData);
        KeyInventory.ChangeItemPosition(SelectDirection.Right);

        yield return interactable.Interact();
    }
}
