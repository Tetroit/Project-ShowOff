using UnityEngine;

public class Lantern : InventoryItemView
{
    [SerializeField] GameObject[] UI_Icons;
    
    public override void Select()
    {
        base.Select();
        foreach (var icon in UI_Icons) icon.SetActive(true);
    }

    public override void Deselect()
    {
        base.Deselect();
        foreach (var icon in UI_Icons) icon.SetActive(false);

    }
}
