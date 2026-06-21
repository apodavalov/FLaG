namespace FLaG.Core.Data.StateMachines
{
    public sealed record RemovingUnreachableStatesIterationPostReport(
        IReadOnlySet<Label> CurrentReachableStates,
        IReadOnlySet<Label> NextReachableStates,
        IReadOnlySet<Label> CurrentApproachedStates,
        IReadOnlySet<Label> CurrentApproachedMinusCurrentReachableStates,
        int Iteration,
        bool IsLastIteration
    );
}
