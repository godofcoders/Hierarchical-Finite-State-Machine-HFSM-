public class RootState : HFSMState
{
    private bool initialized;

    public RootState(HFSMStateMachine machine, HFSMContext context, HFSMState parent)
        : base(machine, context, parent) { }

    public override void Enter()
    {
        if (initialized) return;

        initialized = true;

        var locomotion = machine.GetState<SubStateMachineState>();
        machine.RequestTransition(locomotion);
    }
}