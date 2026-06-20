using System.Collections.Immutable;
using FLaGLib.Data.RegExps;
using FLaGLib.Extensions;
using RegExpBinaryUnion = FLaGLib.Data.RegExps.BinaryUnion;

namespace FLaGLib.Data.Languages
{
    [ComparableEquatable]
    public sealed partial class Union : Entity
    {
        public IImmutableSet<Entity> EntityCollection { get; }

        public Union(IEnumerable<Entity> entities)
        {
            EntityCollection = entities.ToImmutableSortedSet();

            if (EntityCollection.Count < 2)
            {
                throw new ArgumentException("Union must have at least two items.");
            }

            _Variables = new Lazy<IImmutableSet<Variable>>(() =>
                CollectVariables(EntityCollection)
            );
        }

        public bool EqualsNonnull(Union other) =>
            EntityCollection.SequenceEqual(other.EntityCollection);

        public int CompareToNonnull(Union other) =>
            EntityCollection.SequenceCompare(other.EntityCollection);

        public override int GetHashCode()
        {
            HashCode hashCode = new();
            foreach (Entity entity in EntityCollection)
            {
                hashCode.Add(entity);
            }
            return hashCode.ToHashCode();
        }

        private readonly Lazy<IImmutableSet<Variable>> _Variables;

        public override IImmutableSet<Variable> Variables => _Variables.Value;

        public override int Priority => 3;

        public override EntityType EntityType => EntityType.Union;

        public override string ToString() =>
            string.Concat("(", string.Join(',', EntityCollection), ")");

        public override Expression ToRegExp()
        {
            Expression? prev = null;

            foreach (Entity entity in EntityCollection)
            {
                if (prev is null)
                {
                    prev = entity.ToRegExp();
                }
                else
                {
                    prev = new RegExpBinaryUnion(prev, entity.ToRegExp());
                }
            }

            return prev!;
        }
    }
}
