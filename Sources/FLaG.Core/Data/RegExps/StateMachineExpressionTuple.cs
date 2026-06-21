using FLaG.Core.Data.StateMachines;

namespace FLaG.Core.Data.RegExps
{
    public sealed record StateMachineExpressionTuple(
        Expression Expression,
        StateMachine StateMachine,
        int Number
    ) { }
}
