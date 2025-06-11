using amogus;
using UnityEngine;

public class Key : InventoryItemView
{
    [SerializeField] string _doorCode;
    [SerializeField] bool _storeInInventory = false;
    public string doorCode => _doorCode;
    static Transform _canvasCache;


    //on instantiate
    public override void AddInInventory(GameObject user)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.keyPickup, transform.position);
        if (!_storeInInventory) return;

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
        rectTransform.localScale = new Vector3(1, 1, 1);

    }
}
