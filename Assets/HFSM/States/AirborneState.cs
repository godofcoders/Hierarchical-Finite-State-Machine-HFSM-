public abstract class AirborneState : HFSMState
{
    public AirborneState(HFSMStateMachine machine, HFSMContext context, HFSMState parent)
        : base(machine, context, parent)
    {
    }
}