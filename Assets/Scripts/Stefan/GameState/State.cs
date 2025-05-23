public abstract class State
{
    protected GameStateManager fsm;
    public void Init(GameStateManager fsm)
    {
        this.fsm = fsm;
    }
    public abstract void Enter();
    public abstract void Exit();
}
