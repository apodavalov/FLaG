using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;

namespace FLaG.Data
{
    abstract class Entity
    {
        public static Entity Load(XmlReader reader, List<Variable> variableCollection)
        {
            while (!reader.IsStartElement()) reader.Read();

            switch (reader.Name)
            {
                case "concat":
                    return new Concat(reader, variableCollection);
                case "degree":
                    return new Degree(reader, variableCollection);
                case "symbol":
                    return new Symbol(reader, variableCollection);
                default:
                    return null; // никогда не случится
            }
        }

        public abstract Entity ToRegularSet();

        public abstract void Save(XmlWriter writer);       
    }
}
