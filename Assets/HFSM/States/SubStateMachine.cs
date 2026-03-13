public class SubStateMachineState : HFSMState
{
    protected HFSMStateMachine subMachine;
    protected HFSMState initialSubState;

    public SubStateMachineState(HFSMStateMachine machine, HFSMContext context, HFSMState parent)
        : base(machine, context, parent)
    {
        subMachine = new HFSMStateMachine();
    }

    public void SetInitialSubState(HFSMState state)
    {
        initialSubState = state;
    }

    public HFSMStateMachine GetSubMachine()
    {
        return subMachine;
    }

    public override void Enter()
    {
        base.Enter();

        if (initialSubState != null)
        {
            subMachine.SetInitialState(initialSubState);
        }
    }

    public override void Tick()
    {
        base.Tick();

        subMachine.Tick();
    }

    public override void Exit()
    {
        base.Exit();
    }
}