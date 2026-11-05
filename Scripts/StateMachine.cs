public class StateMachine
{
    private IState mCurrent;

    public void ChangeState(IState next)
    {
        mCurrent?.OnExit();
        mCurrent = next;
        mCurrent?.OnEnter();
    }

    public void Update()
    {
        mCurrent?.OnUpdate();
    }
}
