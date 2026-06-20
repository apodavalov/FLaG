namespace FLaGLib.Data.StateMachines
{
    public sealed record RemovingUnreachableStatesBeginPostReport(
        IReadOnlySet<Label> ReachableStates,
        IReadOnlySet<Label> ApproachedStates,
        int Iteration
    );
}
