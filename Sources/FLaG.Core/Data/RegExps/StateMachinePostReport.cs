namespace FLaG.Core.Data.RegExps
{
    public sealed record StateMachinePostReport(
        StateMachineExpressionTuple New,
        IReadOnlyList<StateMachineExpressionWithOriginal> Dependencies
    ) { }
}
