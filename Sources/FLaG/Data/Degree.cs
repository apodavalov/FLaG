using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;

namespace FLaG.Data
{
    class Degree : Entity
    {
        public Degree() 
            : base()
        {

        }
		
		public override Entity DeepClone()
		{
			Degree d = new Degree();
			
			d.Base = Base.DeepClone();
			d.Exp = Exp.DeepClone();
			
			d.NumLabel = NumLabel;
			
			return d;
		}

        public Degree(XmlReader reader, List<Variable> variableCollection) 
            : this()
        {
            while (!reader.IsStartElement("degree")) reader.Read();

            reader.ReadStartElement(); // degree

            while (reader.IsStartElement())
            {
                string name = reader.Name;
                
                reader.ReadStartElement(); // base or exp

                if (reader.IsStartElement())
                {
                    if (name == "base")
                        Base = Entity.Load(reader, variableCollection);
                    else if (name == "exp")
                        Exp = Quantity.Load(reader, variableCollection);
                }

                reader.ReadEndElement(); // base or exp
            }

            reader.ReadEndElement(); // degree
        }

        public Entity Base
        {
            get;
            set;
        }

        public Quantity Exp
        {
            get;
            set;
        }

        public override Entity ToRegularSet()
        {
            if (Base is Symbol && Exp is Number)
            {
                Concat c = new Concat();
                Symbol s = (Symbol)Base;
                Number n = (Number)Exp;

                for (int i = 0; i < n.Value; i++)
                    c.EntityCollection.Add(s.DeepClone());

                return c;
            }
            else
            {
                Degree d = new Degree();
                d.Base = Base.ToRegularSet();
                d.Exp = Exp;
                return d;
            }
        }
		
		public override Entity ToRegularExp()
		{
			if (Exp is Variable)
			{
				Variable v = (Variable)Exp;
				Concat c = new Concat();
				Entity e = Base.ToRegularExp();
				
				bool needClone = true;
				
				if (v.Num == 1)		
					c.EntityCollection.Add(e);		
				else if (v.Num > 1)
				{
					Degree d = new Degree();
					d.Base = e;
					Number n = new Number();					
					n.Value = v.Num;
					d.Exp = n;
					c.EntityCollection.Add(d);
				}
				else
					needClone = false;
				
				Repeat r = new Repeat();
				r.AtLeastOne = v.Sign == SignEnum.MoreThanZero;
				r.Entity = needClone ? e.DeepClone() : e;
				
				c.EntityCollection.Add(r);
				
				if (c.EntityCollection.Count == 1)
					return c.EntityCollection[0];
				else
					return c;
			}
			else				
			{
				Degree d = new Degree();
				d.Base = this.Base.ToRegularExp();
				d.Exp = this.Exp;
				
				return d;
			}
			
		}

        public override void Save(Writer writer)
        {
			writer.Write("{");
			writer.Write(@"\left\{");
			Base.Save(writer);
			writer.Write(@"\right\}");
			writer.Write("^");
            Exp.Save(writer);
			writer.Write("}");
        }
	
		public override void SaveAsRegularExp(Writer writer, bool full)
		{
			if (full)
				writer.Write(@"{\underbrace");
			
			writer.Write("{");
			
			if (Base is Symbol || Base is Concat)
				Base.SaveAsRegularExp(writer,full);
			else
			{
				writer.Write(@"\left(");
				Base.SaveAsRegularExp(writer,full);
				writer.Write(@"\right)");
			}					
			writer.Write("^");
            Exp.SaveAsRegularExp(writer);
			writer.Write("}");
			
			if (full)
			{
				writer.Write(@"_\text{");
				writer.Write(NumLabel);
				writer.Write(@"}}");
			}
		}

		public override int MarkDeepest(int val)
		{
			if (NumLabel != null)
				return val;
			
			int oldval = val;
			
			val = Base.MarkDeepest(val);
			
			if (oldval == val)
			{
				NumLabel = val;
				val++;
			}
			
			return val;
		}
    }
}
