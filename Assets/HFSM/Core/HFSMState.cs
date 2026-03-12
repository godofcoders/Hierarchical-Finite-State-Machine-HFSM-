using System.Collections.Generic;

public abstract class HFSMState
{
    protected HFSMStateMachine machine;
    public HFSMContext context;
    protected HFSMState parent;

    private List<HFSMTransition> transitions = new List<HFSMTransition>();
    public HFSMState Parent => parent;

    private List<HFSMState> children = new();

    public IReadOnlyList<HFSMState> Children => children;

    public void AddChild(HFSMState child)
    {
        children.Add(child);
    }

    public IEnumerable<HFSMTransition> GetTransitions()
    {
        return transitions;
    }

    public HFSMState(HFSMStateMachine machine, HFSMContext context, HFSMState parent)
    {
        this.machine = machine;
        this.context = context;
        this.parent = parent;
    }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void Tick()
    {
        parent?.Tick();
    }

    public void AddTransition(HFSMState targetState, TransitionGuard guard, int priority = 0)
    {
        transitions.Add(new HFSMTransition(targetState, guard, priority));
    }

    public HFSMState CheckTransitions()
    {
        HFSMTransition bestTransition = null;

        for (int i = 0; i < transitions.Count; i++)
        {
            var transition = transitions[i];
            if (transition.ShouldTransition(context))
            {
                if (bestTransition == null || transition.Priority > bestTransition.Priority)
                {
                    bestTransition = transition;
                }
            }
        }

        return bestTransition?.TargetState;
    }
}