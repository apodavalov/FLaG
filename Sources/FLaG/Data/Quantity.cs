﻿using FLaG.Output;
using System.Collections.Generic;
using System.Xml;

namespace FLaG.Data
{
    abstract class Quantity
    {
        public static Quantity Load(XmlReader reader, List<Variable> variableCollection)
        {
            while (!reader.IsStartElement()) reader.Read();

            switch (reader.Name)
            {
                case "number":
                    return new Number(reader, variableCollection);
                case "vref":
                    reader.ReadStartElement();
                    Variable var = new Variable();
                    var.Name = reader.ReadContentAsString()[0];
                    var = variableCollection[variableCollection.BinarySearch(var)];
                    reader.ReadEndElement();
                    return var;
                default:
                    return null; // никогда не случится
            }
        }
		
		public abstract Quantity DeepClone();
		
		public abstract void SaveAsRegularExp(Writer writer);
		
        public abstract void Save(Writer writer);
    }
}
