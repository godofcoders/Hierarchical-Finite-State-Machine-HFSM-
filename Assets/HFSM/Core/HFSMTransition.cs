public class HFSMTransition
{
    public HFSMState TargetState;
    public TransitionGuard Guard;
    public int Priority;

    public HFSMTransition(HFSMState target, TransitionGuard guard, int priority)
    {
        TargetState = target;
        Guard = guard;
        Priority = priority;
    }

    public bool ShouldTransition(HFSMContext context)
    {
        return Guard(context);
    }
}