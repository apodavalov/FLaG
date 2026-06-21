namespace FLaG.Core.Data.Grammars
{
    public sealed record BeginPostReport<T>(int Iteration, IReadOnlySet<T> SymbolSet)
        where T : Symbol;
}
