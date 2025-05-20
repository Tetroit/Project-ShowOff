using amogus;
using UnityEngine;

public class Key : InventoryItemView
{
    [SerializeField] string _doorCode;

    public override void Interact(GameObject user)
    {
        base.Interact(user);
        Collider[] hits = Physics.OverlapSphere(user.transform.position, 1,int.MaxValue, QueryTriggerInteraction.Collide);
        if (hits.Length == 0) return;
        DoorCutsceneTrigger door = null;
        foreach (Collider c in hits)
            if (c.transform.TryGetComponent(out door)) break;

        if(door == null || !door.isLocked || door.unlockCode != _doorCode) return;

        door.isLocked = false;
    }

    public override void AddInInventory(GameObject user)
    {
        Debug.Log("Spawn in inventory");
    }
}
