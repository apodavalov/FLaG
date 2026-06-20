namespace FLaGLib.Data.Grammars
{
    public sealed record IterationPostReport<T>(
        int Iteration,
        IReadOnlySet<T> PreviousSymbolSet,
        IReadOnlySet<T> NewSymbolSet,
        IReadOnlySet<T> NextSymbolSet,
        bool IsLastIteration
    )
        where T : Symbol;
}
