using System;
using System.Collections.Generic;

public class HFSMStateMachine
{
    private List<HFSMState> activeStates = new();
    private HFSMState requestedState;

    public HFSMState CurrentState => activeStates.Count > 0 ? activeStates[^1] : null;
    private List<HFSMTransition> globalTransitions = new List<HFSMTransition>();
    private Dictionary<Type, HFSMState> stateRegistry = new();
    public void SetInitialState(HFSMState state)
    {
        activeStates.Clear();

        Stack<HFSMState> stack = new();

        HFSMState current = state;

        while (current != null)
        {
            stack.Push(current);
            current = current.Parent;
        }

        while (stack.Count > 0)
        {
            var s = stack.Pop();
            activeStates.Add(s);
            s.Enter();
        }
    }

    public void AddGlobalTransition(HFSMState targetState, TransitionGuard guard, int priority = 0)
    {
        globalTransitions.Add(new HFSMTransition(targetState, guard, priority));
    }

    private HFSMState CheckGlobalTransitions()
    {
        if (CurrentState == null)
            return null;

        foreach (var transition in globalTransitions)
        {
            if (transition.ShouldTransition(CurrentState.context))
            {
                return transition.TargetState;
            }
        }

        return null;
    }

    public void Tick()
    {
        if (CurrentState == null)
            return;

        HFSMState transition = CheckGlobalTransitions();

        if (transition == null)
        {
            transition = CheckHierarchicalTransitions();
        }

        if (transition != null)
        {
            RequestTransition(transition);
        }

        var statesSnapshot = new List<HFSMState>(activeStates);

        for (int i = 0; i < statesSnapshot.Count; i++)
        {
            statesSnapshot[i].Tick();
        }

        if (requestedState != null)
        {
            ChangeState(requestedState);
            requestedState = null;
        }
    }

    private HFSMState CheckHierarchicalTransitions()
    {
        for (int i = activeStates.Count - 1; i >= 0; i--)
        {
            var state = activeStates[i];

            HFSMState transition = state.CheckTransitions();

            if (transition != null)
                return transition;
        }

        return null;
    }

    public void ChangeState(HFSMState newState)
    {
        if (newState == null)
            return;

        List<HFSMState> newPath = new();

        HFSMState current = newState;

        while (current != null)
        {
            newPath.Insert(0, current);
            current = current.Parent;
        }

        int commonIndex = 0;

        while (commonIndex < activeStates.Count &&
               commonIndex < newPath.Count &&
               activeStates[commonIndex] == newPath[commonIndex])
        {
            commonIndex++;
        }

        for (int i = activeStates.Count - 1; i >= commonIndex; i--)
        {
            var state = activeStates[i];

            if (!newPath.Contains(state))
            {
                state.Exit();
                activeStates.RemoveAt(i);
            }
        }

        for (int i = commonIndex; i < newPath.Count; i++)
        {
            if (!activeStates.Contains(newPath[i]))
            {
                activeStates.Add(newPath[i]);
                newPath[i].Enter();
            }
        }
    }

    public void RequestTransition(HFSMState state)
    {
        if (state == null)
            return;

        requestedState = state;
    }

    public void RegisterState(HFSMState state)
    {
        Type type = state.GetType();

        if (stateRegistry.ContainsKey(type))
        {
            throw new Exception($"State already registered: {type}");
        }

        stateRegistry.Add(type, state);
    }

    public T GetState<T>() where T : HFSMState
    {
        if (stateRegistry.TryGetValue(typeof(T), out var state))
            return (T)state;

        throw new Exception($"State not registered: {typeof(T)}");
    }
    public IEnumerable<HFSMState> GetAllStates()
    {
        return stateRegistry.Values;
    }

    public T CreateState<T>(HFSMContext context, HFSMState parent) where T : HFSMState
    {
        var state = (T)Activator.CreateInstance(typeof(T), this, context, parent);

        RegisterState(state);

        return state;
    }
}