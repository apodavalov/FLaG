namespace FLaGLib.Data.StateMachines
{
    public sealed record MinimizingIterationPostReport(
        SetsOfEquivalence SetsOfEquivalence,
        int Iteration,
        bool IsLastIteration
    );
}
