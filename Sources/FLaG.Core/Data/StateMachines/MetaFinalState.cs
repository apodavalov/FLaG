using System.Collections.Immutable;

namespace FLaG.Core.Data.StateMachines
{
    public sealed class MetaFinalState(
        IEnumerable<Label> requiredStates,
        IEnumerable<Label> optionalStates
    )
    {
        public IImmutableSet<Label> RequiredStates { get; } = requiredStates.ToImmutableSortedSet();

        public IImmutableSet<Label> OptionalStates { get; } = optionalStates.ToImmutableSortedSet();
    }
}
