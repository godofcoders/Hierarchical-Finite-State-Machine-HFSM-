public static class FSMGraphBuilder
{
    public static HFSMStateMachine Build(HFSMContext context)
    {
        HFSMStateMachine machine = new HFSMStateMachine();

        // 1. ROOT
        RootState root = machine.CreateState<RootState>(context, null);

        // 2. BUILD SUB-GRAPHS (Passing references downward)
        var locomotion = BuildLocomotion(machine, context, root);
        var attack = BuildCombat(machine, context, root, locomotion);

        // 3. GLOBAL SETUP
        SetupGlobals(machine, context, root);

        // 4. INITIAL STATE
        machine.SetInitialState(root);

        return machine;
    }

    private static SubStateMachineState BuildLocomotion(HFSMStateMachine machine, HFSMContext context, RootState root)
    {
        var locomotion = machine.CreateState<SubStateMachineState>(context, root);
        HFSMStateMachine locomotionMachine = locomotion.GetSubMachine();

        /* --- CREATE STATES --- */
        var idle = locomotionMachine.CreateState<IdleState>(context, locomotion);
        var walk = locomotionMachine.CreateState<WalkState>(context, locomotion);
        var jump = locomotionMachine.CreateState<JumpState>(context, locomotion);

        /* --- WIRE TRANSITIONS --- */
        idle.AddTransition(walk, ctx => ctx.MoveInput != 0, 10);
        walk.AddTransition(idle, ctx => ctx.MoveInput == 0, 10);

        idle.AddTransition(jump, ctx => ctx.JumpPressed, 30);
        walk.AddTransition(jump, ctx => ctx.JumpPressed, 30);

        /* --- SET INITIALS --- */
        locomotionMachine.SetInitialState(idle);
        locomotion.SetInitialSubState(idle);

        return locomotion; // Return the reference so other domains can transition to it
    }

    private static AttackState BuildCombat(HFSMStateMachine machine, HFSMContext context, RootState root, SubStateMachineState locomotion)
    {
        var attack = machine.CreateState<AttackState>(context, root);

        /* --- WIRE TRANSITIONS --- */
        // Transition back to the locomotion state reference we passed in
        attack.AddTransition(locomotion, ctx => !ctx.AttackPressed, 10);

        return attack;
    }

    private static void SetupGlobals(HFSMStateMachine machine, HFSMContext context, RootState root)
    {
        var death = machine.CreateState<DeathState>(context, root);

        /* --- WIRE GLOBAL TRANSITIONS --- */
        machine.AddGlobalTransition(death, ctx => ctx.ForceDeath, 100);
    }
}