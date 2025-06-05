public readonly struct InputFacade
{
    readonly InputSystem_Actions _input;

    public InputFacade(InputSystem_Actions input)
    {
        _input = input;
    }

    public InputSystem_Actions.UIActions UI => _input.UI;
    public InputSystem_Actions.PlayerActions Player => _input.Player;
    
}
