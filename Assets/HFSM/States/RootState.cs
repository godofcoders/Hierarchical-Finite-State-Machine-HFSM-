public class RootState : HFSMState
{
    public RootState(HFSMStateMachine machine, HFSMContext context, HFSMState parent)
        : base(machine, context, parent)
    {
    }

    public override void Enter()
    {
        var locomotion = machine.GetState<LocomotionState>();
        var combat = machine.GetState<CombatState>();

        machine.ChangeState(locomotion);
        machine.ChangeState(machine.GetState<IdleState>());
        machine.ChangeState(combat);
    }
}