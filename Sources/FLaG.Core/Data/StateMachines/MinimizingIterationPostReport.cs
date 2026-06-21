namespace FLaG.Core.Data.StateMachines
{
    public sealed record MinimizingIterationPostReport(
        SetsOfEquivalence SetsOfEquivalence,
        int Iteration,
        bool IsLastIteration
    );
}
