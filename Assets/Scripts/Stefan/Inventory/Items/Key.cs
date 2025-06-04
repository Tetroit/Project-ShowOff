using amogus;
using UnityEngine;

public class Key : InventoryItemView
{
    [SerializeField] string _doorCode;
    public string doorCode => _doorCode;
    static Transform _canvasCache;

    public override void Interact(GameObject user)
    {
        base.Interact(user);
        Collider[] hits = Physics.OverlapSphere(user.transform.position, 1,int.MaxValue, QueryTriggerInteraction.Collide);
        if (hits.Length == 0) return;
        DoorTrigger door = null;
        foreach (Collider c in hits)
            if (c.transform.TryGetComponent(out door)) break;

        if(door == null || !door.isLocked || door.unlockCode != _doorCode) return;

        //door.Unlock(); 
    }

    //on instantiate
    public override void AddInInventory(GameObject user)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.keyPickup, transform.position);

        //get canvas
        //spawn itself in the canvas
        if(_canvasCache == null)
        {
            _canvasCache = FindFirstObjectByType<Canvas>().transform.FindDeepChild("KeyInventory");
        }
        transform.SetParent(_canvasCache);

        RectTransform rectTransform = transform as RectTransform;
        rectTransform.anchorMax = new Vector2(1,0);
        rectTransform.anchorMin = new Vector2(1,0);
        rectTransform.pivot = new Vector2(1,0);

        rectTransform.anchoredPosition = new Vector2(0,0);

    }
}
