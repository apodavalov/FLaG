using System;
using FLaGLib.Extensions;

namespace FLaGLib.Data.Helpers
{
    public class Tree<E,C> where C : TreeCollection<E, C>
    {
        public E Entry
        {
            get;
            private set;
        }

        public C Subtrees
        {
            get;
            private set;
        }

        public Tree(E entry, C subtrees = null)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            Entry = entry;
            Subtrees = subtrees;
        }
    }
}
