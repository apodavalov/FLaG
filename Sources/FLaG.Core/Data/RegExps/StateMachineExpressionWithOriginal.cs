namespace FLaG.Core.Data.RegExps
{
    public sealed record StateMachineExpressionWithOriginal(
        StateMachineExpressionTuple StateMachineExpression,
        StateMachineExpressionTuple? OriginalStateMachineExpression = null
    ) { }
}
