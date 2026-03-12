public static class FSMGraphBuilder
{
    public static HFSMStateMachine Build(HFSMContext context)
    {
        HFSMStateMachine machine = new HFSMStateMachine();

        // ROOT
        var root = machine.CreateState<RootState>(context, null);

        // PARALLEL REGIONS
        var locomotion = machine.CreateState<LocomotionState>(context, root);
        var combat = machine.CreateState<CombatState>(context, root);

        // LOCOMOTION STATES
        var idle = machine.CreateState<IdleState>(context, locomotion);
        var walk = machine.CreateState<WalkState>(context, locomotion);
        var jump = machine.CreateState<JumpState>(context, locomotion);

        // COMBAT STATE
        var attack = machine.CreateState<AttackState>(context, combat);

        // GLOBAL STATE
        var death = machine.CreateState<DeathState>(context, root);

        //-----------------------------------
        // TRANSITION GUARDS
        //-----------------------------------

        TransitionGuard move = ctx => ctx.MoveInput != 0;
        TransitionGuard stop = ctx => ctx.MoveInput == 0;
        TransitionGuard jumpPressed = ctx => ctx.JumpPressed && ctx.IsGrounded;
        TransitionGuard attackPressed = ctx => ctx.AttackPressed;
        TransitionGuard deathGuard = ctx => ctx.ForceDeath;

        //-----------------------------------
        // GLOBAL TRANSITIONS
        //-----------------------------------

        machine.AddGlobalTransition(death, deathGuard, 100);

        //-----------------------------------
        // LOCOMOTION TRANSITIONS
        //-----------------------------------

        idle.AddTransition(walk, move, 10);
        walk.AddTransition(idle, stop, 10);

        idle.AddTransition(jump, jumpPressed, 30);
        walk.AddTransition(jump, jumpPressed, 30);

        //-----------------------------------
        // COMBAT TRANSITIONS
        //-----------------------------------

        combat.AddTransition(attack, attackPressed, 40);

        //-----------------------------------
        // INITIAL STATE
        //-----------------------------------

        machine.SetInitialState(root);

        return machine;
    }
}