namespace FLaGLib.Data.StateMachines
{
    public sealed record MinimizingBeginPostReport(
        SetsOfEquivalence SetsOfEquivalence,
        int Iteration
    );
}
