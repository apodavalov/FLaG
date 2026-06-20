using System.Globalization;

namespace FLaGLib.Data.StateMachines
{
    [ComparableEquatable]
    public sealed partial class Transition(Label currentState, char symbol, Label nextState)
    {
        public Label CurrentState { get; } = currentState;

        public Label NextState { get; } = nextState;

        public char Symbol { get; } = symbol;

        public override int GetHashCode() => HashCode.Combine(CurrentState, NextState, Symbol);

        public bool EqualsNonnull(Transition other) =>
            CurrentState.Equals(other.CurrentState)
            && NextState.Equals(other.NextState)
            && Symbol.Equals(other.Symbol);

        public int CompareToNonnull(Transition other)
        {
            int result = CurrentState.CompareTo(other.CurrentState);

            if (result != 0)
            {
                return result;
            }

            result = Symbol.CompareTo(other.Symbol);

            if (result != 0)
            {
                return result;
            }

            return NextState.CompareTo(other.NextState);
        }

        public override string ToString() =>
            string.Format(
                CultureInfo.InvariantCulture,
                "δ({0},{1}) -> {2}",
                CurrentState,
                Symbol,
                NextState
            );
    }
}
