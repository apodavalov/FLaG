using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;
using FLaG.Data.Grammars;

namespace FLaG.Data
{
    abstract class Entity
    {
		public int? NumLabel
		{
			get;
			set;
		}
		
		public Grammar Grammar
		{
			get;
			set;
		}
		
		public abstract void GenerateGrammar(Writer writer, bool isLeft, ref int LastUseNumber, ref int AddionalGrammarsNum);
		
		public abstract Entity DeepClone();
		
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
		
		public abstract Symbol[] CollectAlphabet();
		
		public abstract int MarkDeepest(int val, List<Entity> list);

        public abstract Entity ToRegularSet();
		
		public abstract Entity ToRegularExp();
		
		public abstract void SaveAsRegularExp(Writer writer, bool full);

        public abstract void Save(Writer writer);       
    }
}
