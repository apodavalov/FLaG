using System.Collections.Immutable;
using System.Text;
using FLaG.Core.Extensions;

namespace FLaG.Core.Data.StateMachines
{
    [ComparableEquatable]
    public sealed partial class MetaTransition(
        IEnumerable<Label> metaCurrentRequiredStates,
        IEnumerable<Label> metaCurrentOptionalStates,
        char symbol,
        IEnumerable<Label> metaNextStates
    )
    {
        public IImmutableSet<Label> CurrentRequiredStates { get; } =
            metaCurrentRequiredStates.ToImmutableSortedSet();

        public IImmutableSet<Label> CurrentOptionalStates { get; } =
            metaCurrentOptionalStates.ToImmutableSortedSet();

        public char Symbol { get; } = symbol;

        public IImmutableSet<Label> NextStates { get; } = metaNextStates.ToImmutableSortedSet();

        public override int GetHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(Symbol);
            foreach (Label label in CurrentRequiredStates)
            {
                hashCode.Add(label);
            }
            foreach (Label label in CurrentOptionalStates)
            {
                hashCode.Add(label);
            }
            foreach (Label label in NextStates)
            {
                hashCode.Add(label);
            }

            return hashCode.ToHashCode();
        }

        public bool EqualsNonnull(MetaTransition other) =>
            CurrentOptionalStates.SequenceEqual(other.CurrentOptionalStates)
            && CurrentRequiredStates.SequenceEqual(other.CurrentRequiredStates)
            && NextStates.SequenceEqual(other.NextStates)
            && Symbol == other.Symbol;

        public int CompareToNonnull(MetaTransition other)
        {
            int result = Symbol.CompareTo(other.Symbol);

            if (result != 0)
            {
                return result;
            }

            result = CurrentOptionalStates.SequenceCompare(other.CurrentOptionalStates);

            if (result != 0)
            {
                return result;
            }

            result = CurrentRequiredStates.SequenceCompare(other.CurrentRequiredStates);

            if (result != 0)
            {
                return result;
            }

            return NextStates.SequenceCompare(other.NextStates);
        }

        public override string ToString()
        {
            StringBuilder sb = new("δ([");

            foreach (Label label in CurrentRequiredStates)
            {
                sb.Append(label.ToString());
                sb.Append(' ');
            }

            sb.Append("q_1 ... q_");
            sb.Append(CurrentOptionalStates.Count);
            sb.Append("], ");
            sb.Append(Symbol);
            sb.Append(") -> [");

            foreach (Label label in NextStates)
            {
                sb.Append(' ');
                sb.Append(label.ToString());
            }

            sb.Append("], q_1 ... q_");
            sb.Append(CurrentOptionalStates.Count);
            sb.Append(" :");

            foreach (Label label in CurrentOptionalStates)
            {
                sb.Append(' ');
                sb.Append(label.ToString());
            }

            return sb.ToString();
        }
    }
}
