namespace FLaG.Core.Data.StateMachines
{
    public sealed record MinimizingBeginPostReport(
        SetsOfEquivalence SetsOfEquivalence,
        int Iteration
    );
}
