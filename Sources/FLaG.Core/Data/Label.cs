using System.Collections.Immutable;
using System.Text;
using FLaG.Core.Extensions;

namespace FLaG.Core.Data
{
    [ComparableEquatable]
    public sealed partial class Label
    {
        public IImmutableSet<SingleLabel> Sublabels { get; }

        public LabelType LabelType { get; }

        public Label(IEnumerable<SingleLabel> sublabels)
        {
            Sublabels = sublabels.ToImmutableSortedSet();
            LabelType = LabelType.Complex;
        }

        public Label(SingleLabel singleLabel)
        {
            Sublabels = [singleLabel];
            LabelType = LabelType.Simple;
        }

        public int FetchHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(LabelType);

            foreach (SingleLabel label in Sublabels)
            {
                hashCode.Add(label);
            }

            return hashCode.ToHashCode();
        }

        public bool EqualsNonnull(Label other) =>
            LabelType.Equals(other.LabelType) && Sublabels.SequenceEqual(other.Sublabels);

        public int CompareToNonnull(Label other)
        {
            int result = LabelType.CompareTo(other.LabelType);

            if (result != 0)
            {
                return result;
            }

            return Sublabels.SequenceCompare(other.Sublabels);
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            if (LabelType == LabelType.Complex)
            {
                builder.Append('[');
            }

            foreach (SingleLabel label in Sublabels)
            {
                builder.Append('{').Append(label).Append('}');
            }

            if (LabelType == LabelType.Complex)
            {
                builder.Append(']');
            }

            return builder.ToString();
        }

        public Label ConvertToComplex()
        {
            if (LabelType != LabelType.Simple)
            {
                throw new InvalidOperationException(
                    "Cannot convert label to complex for non simple label type."
                );
            }

            return new Label(Sublabels);
        }

        public SingleLabel ExtractSingleLabel()
        {
            if (LabelType != LabelType.Simple)
            {
                throw new InvalidOperationException(
                    "Cannot extract single label from non simple label type."
                );
            }

            return Sublabels.Single();
        }
    }
}
