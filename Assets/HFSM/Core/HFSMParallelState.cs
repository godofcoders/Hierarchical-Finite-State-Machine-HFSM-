using System.Collections.Generic;

public class HFSMParallelState : HFSMState
{
    private List<HFSMState> regions = new();

    public HFSMParallelState(HFSMStateMachine machine, HFSMContext context, HFSMState parent)
        : base(machine, context, parent)
    {
    }

    public void AddRegion(HFSMState state)
    {
        regions.Add(state);
        AddChild(state);
    }

    public override void Enter()
    {
        foreach (var region in regions)
        {
            machine.ChangeState(region);
        }
    }

    public override void Exit()
    {
        foreach (var region in regions)
        {
            region.Exit();
        }
    }
}