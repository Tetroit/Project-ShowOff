using amogus;
using System.Linq;
using UnityEngine;

public class Key : InventoryItemView
{
    [SerializeField] string _doorCode;

    public override void Interact(GameObject user)
    {
        base.Interact(user);

        RaycastHit[] hits = Physics.RaycastAll(user.transform.position, user.transform.forward, 1);
        if (hits.Length == 0) return;
        Door door = null;
        foreach (RaycastHit h in hits)
            if (h.transform.TryGetComponent(out door)) ;

        if(door == null || door.unlockCode != _doorCode) return;

        door.Open();
    }
}
