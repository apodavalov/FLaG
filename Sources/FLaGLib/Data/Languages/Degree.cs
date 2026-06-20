using System.Collections.Immutable;
using System.Text;
using FLaGLib.Data.RegExps;

namespace FLaGLib.Data.Languages
{
    [ComparableEquatable]
    public sealed partial class Degree : Entity
    {
        public Entity Entity { get; }

        public Exponent Exponent { get; }

        public Degree(Entity entity, Exponent exponent)
        {
            Entity = entity;
            Exponent = exponent;

            _Variables = new Lazy<IImmutableSet<Variable>>(CollectVariables);
        }

        private ImmutableSortedSet<Variable> CollectVariables()
        {
            var variables = ImmutableSortedSet.CreateBuilder<Variable>();

            if (Exponent is Variable variable)
            {
                variables.Add(variable);
            }

            variables.UnionWith(Entity.Variables);

            return variables.ToImmutable();
        }

        public bool EqualsNonnull(Degree other) =>
            Entity.Equals(other.Entity) && Exponent.Equals(other.Exponent);

        public int CompareToNonnull(Degree other)
        {
            int result = Entity.CompareTo(other.Entity);

            if (result != 0)
            {
                return result;
            }

            return Exponent.CompareTo(other.Exponent);
        }

        public override int GetHashCode() => HashCode.Combine(Entity, Exponent);

        private readonly Lazy<IImmutableSet<Variable>> _Variables;

        public override IImmutableSet<Variable> Variables => _Variables.Value;

        public override int Priority => 1;

        public override EntityType EntityType => EntityType.Degree;

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.Append(Entity.ToString());

            sb.Append("^(");
            sb.Append(Exponent.ToString());
            sb.Append(')');

            return sb.ToString();
        }

        public override Expression ToRegExp() => Exponent.ToRegExp(Entity);
    }
}
