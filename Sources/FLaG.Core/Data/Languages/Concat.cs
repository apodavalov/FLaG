using System.Collections.Immutable;
using FLaG.Core.Data.RegExps;
using FLaG.Core.Extensions;
using RegExpBinaryConcat = FLaG.Core.Data.RegExps.BinaryConcat;

namespace FLaG.Core.Data.Languages
{
    [ComparableEquatable]
    public sealed partial class Concat : Entity
    {
        public IImmutableList<Entity> EntityCollection { get; }

        public Concat(IEnumerable<Entity> entities)
        {
            EntityCollection = entities.ToImmutableList();
            if (EntityCollection.Count < 2)
            {
                throw new ArgumentException("Concat must have at least two items.");
            }
            _Variables = new Lazy<IImmutableSet<Variable>>(() =>
                CollectVariables(EntityCollection)
            );
        }

        public bool EqualsNonnull(Concat other) =>
            EntityCollection.SequenceEqual(other.EntityCollection);

        public int CompareToNonnull(Concat other) =>
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

        public override int Priority => 2;

        public override EntityType EntityType => EntityType.Concat;

        public override string ToString() =>
            string.Concat("(", string.Concat(EntityCollection), ")");

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
                    prev = new RegExpBinaryConcat(prev, entity.ToRegExp());
                }
            }

            return prev!;
        }
    }
}
