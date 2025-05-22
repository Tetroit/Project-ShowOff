using UnityEngine;

public class Book : InventoryItemView
{
    [SerializeField] GameStateManager _gameStateManager;
    public override void Select()
    {
        base.Select();
        if( _gameStateManager != null)
        _gameStateManager.SwitchState(GameState.UI);
    }

    public override void Deselect()
    {
        base.Deselect();
        if( _gameStateManager != null)
        _gameStateManager.SwitchState(GameState.Play);

    }
}
