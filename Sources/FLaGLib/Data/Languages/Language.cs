using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Data.Languages
{
    public class Language
    {
        public Entity Entity
        {
            get;
            private set;
        }

        public IReadOnlySet<Variable> Variables
        {
            get
            {
                return Entity.Variables;
            }
        }

        public Language(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Entity = entity;
        }

        public Tree Split()
        {
            throw new NotImplementedException();
        }
    }
}
