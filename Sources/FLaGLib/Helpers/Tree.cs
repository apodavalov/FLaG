namespace FLaGLib.Helpers
{
    public class Tree<E, C>(E entry, C? subtrees = null)
        where C : TreeCollection<E, C>
    {
        public E Entry { get; } = entry;

        public C? Subtrees { get; } = subtrees;
    }
}
