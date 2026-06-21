using System.Collections.Immutable;
using FLaG.Core.Data.RegExps;
using RegExpSymbol = FLaG.Core.Data.RegExps.Symbol;

namespace FLaG.Core.Data.Languages
{
    [ComparableEquatable]
    public sealed partial class Symbol(char character) : Entity
    {
        public char Character { get; } = character;

        public bool EqualsNonnull(Symbol other) => Character.Equals(other.Character);

        public override int GetHashCode() => Character.GetHashCode();

        public int CompareToNonnull(Symbol other) => Character.CompareTo(other.Character);

        private readonly Lazy<IImmutableSet<Variable>> _Variables = new(() => []);

        public override IImmutableSet<Variable> Variables => _Variables.Value;

        public override int Priority => 0;

        public override EntityType EntityType => EntityType.Symbol;

        public override string ToString() => Character.ToString();

        public override Expression ToRegExp() => new RegExpSymbol(Character);
    }
}
