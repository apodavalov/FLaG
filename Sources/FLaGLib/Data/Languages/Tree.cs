using System;

namespace FLaGLib.Data.Languages
{
    public class Tree
    {
        public Language Language
        {
            get;
            private set;
        }

        public TreeCollection Subtrees
        {
            get;
            private set;
        }

        public Tree(Language language, TreeCollection subtrees = null)
        {
            if (language == null)
            {
                throw new ArgumentNullException("language");
            }

            Language = language;
        }
    }
}
