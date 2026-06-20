using FLaGLib.Data.StateMachines;

namespace FLaGLib.Data.RegExps
{
    public sealed record StateMachineExpressionTuple(
        Expression Expression,
        StateMachine StateMachine,
        int Number
    ) { }
}
