using System;

namespace FLaGLib.Data.Languages
{
    public class Tree
    {
        public Entity Entity
        {
            get;
            private set;
        }

        public TreeCollection Subtrees
        {
            get;
            private set;
        }

        public Tree(Entity entity, TreeCollection subtrees = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Entity = entity;
        }
    }
}
