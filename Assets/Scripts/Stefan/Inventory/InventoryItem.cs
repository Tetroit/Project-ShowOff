using UnityEngine;

public abstract class InventoryItem : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite InventorySprite { get; private set; }

    public abstract void Interact(GameObject user);
}
