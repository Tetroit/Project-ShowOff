using UnityEngine;
using UnityEngine.UI;

public class InventoryItemView : MonoBehaviour
{
    public InventoryItem Item => _item;
    InventoryItem _item;

    [SerializeField] Image _itemImage;

    public void SetItem(InventoryItem item)
    {
        _item = item;

        _itemImage.sprite = _item.InventorySprite;
    }

    public void Select()
    {
    }

    public void Deselect()
    {

    }
}
