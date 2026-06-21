using System.Collections.Immutable;
using FLaG.Core.Data.RegExps;

namespace FLaG.Core.Data.Languages
{
    [ComparableEquatable]
    public abstract partial class Entity
    {
        public abstract override int GetHashCode();

        public abstract IImmutableSet<Variable> Variables { get; }

        public abstract int Priority { get; }

        public abstract EntityType EntityType { get; }

        protected static IImmutableSet<Variable> CollectVariables(IEnumerable<Entity> entities) =>
            entities.SelectMany(entity => entity.Variables).ToImmutableSortedSet();

        public abstract Expression ToRegExp();

        public virtual bool EqualsNonnull(Entity other) => EntityType.Equals(other.EntityType);

        public virtual int CompareToNonnull(Entity other) => EntityType.CompareTo(other.EntityType);
    }
}
