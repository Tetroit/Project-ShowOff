using UnityEngine;

[CreateAssetMenu(menuName = "Stefan/InventoryItems/BasementKey")]
public class BasementKey : InventoryItem
{
    public override void Interact(GameObject user)
    {
        Debug.Log("Using key " + Name);
    }
}