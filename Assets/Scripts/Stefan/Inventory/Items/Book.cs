using UnityEngine;

public class Book : InventoryItemView
{
    [SerializeField] GameStateManager _gameStateManager;
    [SerializeField] GameObject[] UI_Icons;
    [SerializeField] InteractionManager _interactionManager;
    public override void Select()
    {
        base.Select();
        if( _gameStateManager != null)
        _gameStateManager.SwitchState(GameState.UI);

        foreach (var icon in UI_Icons) icon.SetActive(true);

        if( _interactionManager != null)
        _interactionManager.enabled = false;
    }

    public override void Deselect()
    {
        base.Deselect();
        if( _gameStateManager != null)
        _gameStateManager.SwitchState(GameState.Play);

        foreach (var icon in UI_Icons) icon.SetActive(false);


        if (_interactionManager != null)
            _interactionManager.enabled = true;
    }
}
