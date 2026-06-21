namespace FLaG.Core.Data.Grammars
{
    public static class GrammarDispatcher
    {
        public static T Dispatch<T>(GrammarType grammarType, Func<T> left, Func<T> right) =>
            grammarType switch
            {
                GrammarType.Left => left(),
                GrammarType.Right => right(),
                _ => throw new InvalidOperationException(
                    string.Format("Grammar type {0} is not supported.", grammarType)
                ),
            };

        public static T Dispatch<T>(GrammarType grammarType, T left, T right) =>
            grammarType switch
            {
                GrammarType.Left => left,
                GrammarType.Right => right,
                _ => throw new InvalidOperationException(
                    string.Format("Grammar type {0} is not supported.", grammarType)
                ),
            };

        public static void Dispatch(GrammarType grammarType, Action left, Action right)
        {
            switch (grammarType)
            {
                case GrammarType.Left:
                    left();
                    break;
                case GrammarType.Right:
                    right();
                    break;
                default:
                    throw new InvalidOperationException(
                        string.Format("Grammar type {0} is not supported.", grammarType)
                    );
            }
        }
    }
}
