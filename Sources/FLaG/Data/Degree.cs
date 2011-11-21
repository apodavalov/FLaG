using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;
using FLaG.Data.Grammars;

namespace FLaG.Data
{
    class Degree : Entity
    {
        public Degree() 
            : base()
        {

        }
		
		public override Symbol[] CollectAlphabet()
		{
			return Base.CollectAlphabet();
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

		public override int MarkDeepest(int val, List<Entity> list)
		{
			if (NumLabel != null)
				return val;
			
			int oldval = val;
			
			val = Base.MarkDeepest(val,list);
			
			if (oldval == val)
			{
				list.Add(this);
				if (Exp is Number)
				{
					Number q = (Number)Exp;
					if (q.Value < 3)
						val++;
					else
						val+=q.Value - 1;					
				}
				else
					val++;
				NumLabel = val-1;
			}
			
			return val;
		}
		
		public override void GenerateGrammar(Writer writer, bool isLeft, ref int LastUseNumber, ref int AddionalGrammarsNum)
		{
			if (!(Exp is Number))
				return;
			
			Number number = (Number)Exp;
			
			int degree = number.Value;
			
			if (degree <= 0)
			{
				// TODO: обработать на всякий случай
			}
			else if (degree == 1)
			{
				// TODO: обработать на всякий случай
			}
			else
			{
				writer.WriteLine(@"\item");
				writer.WriteLine(@"Выполним построение в два этапа.",true);
				writer.WriteLine(@"\begin{enumerate}");				
				writer.WriteLine(@"\item");
				writer.WriteLine(@"Для выражения",true);
				writer.WriteLine(@"\begin{math}");
				SaveAsRegularExp(writer,false);
				writer.WriteLine();
				writer.WriteLine(@"\end{math}");
				writer.Write(@", которое является конкатенацией ",true);
				writer.Write(degree);
				writer.WriteLine(@" выражений вида",true);
				writer.WriteLine(@"\begin{math}");
				Base.SaveAsRegularExp(writer,false);
				writer.WriteLine();
				writer.WriteLine(@"\end{math}");
				writer.WriteLine(@"и для которых построены грамматики ",true);
				
				Concat concat = new Concat();
				concat.EntityCollection.Add(Base);
				
				for (int i = 1; i < degree; i++)
				{
					Entity e = Base.DeepClone();
					e.Grammar = Base.Grammar.MakeMirror(ref LastUseNumber, ref AddionalGrammarsNum);
					concat.EntityCollection.Add(e);
				}
				
				concat.NumLabel = NumLabel;

				writer.WriteLine(@"\begin{math}");
				for (int i = 0; i < concat.EntityCollection.Count; i++)
				{
					if (i != 0)
						writer.Write(@",");
					
					concat.EntityCollection[i].Grammar.SaveG(writer);
				}
				writer.WriteLine();
				writer.WriteLine(@"\end{math}");
				writer.WriteLine(@"построим грамматику");
				
				writer.WriteLine(@"\begin{math}");
				// Создаем временно грамматику
				Grammar = new Grammar();
				Grammar.IsLeft = isLeft;
				Grammar.Number = NumLabel.Value;
				Grammar.TargetSymbol = Unterminal.GetInstance(Grammar.Number);
				Grammar.SaveG(writer);
				Grammar = null;
				writer.WriteLine(@"\end{math}.");
				
				writer.WriteLine(@"Эти регулярные грамматики хоть и являются одинаковыми, но порождаются", true);
				writer.WriteLine(@"разными грамматиками, поэтому", true);
				if (concat.EntityCollection.Count > 2)
					writer.WriteLine("грамматики", true);
				else
					writer.WriteLine("грамматика", true);
				
				writer.WriteLine(@"\begin{math}");
				for (int i = 1; i < concat.EntityCollection.Count; i++)
				{
					if (i != 0)
						writer.Write(@",");
					
					concat.EntityCollection[i].Grammar.SaveG(writer);
				}
				writer.WriteLine();
				writer.WriteLine(@"\end{math}");
				
				if (concat.EntityCollection.Count > 2)
					writer.WriteLine("получены",true);
				else
					writer.WriteLine("получена",true);
				
				writer.WriteLine(@"из грамматики");
				
				writer.WriteLine(@"\begin{math}");
				Base.Grammar.SaveG(writer);
				writer.WriteLine();
				writer.WriteLine(@"\end{math}");
				
				writer.WriteLine("с помощью соответствующей заменой индексов, т.е.", true);
				
				
				for (int i = 1; i < concat.EntityCollection.Count; i++)
				{
					writer.WriteLine(@"\begin{math}");						
					concat.EntityCollection[i].Grammar.SaveCortege(writer);					
					writer.WriteLine();	
					writer.WriteLine(@"\end{math}");	
					writer.WriteLine(", где",true);
					writer.WriteLine(@"\begin{math}");						
					concat.EntityCollection[i].Grammar.SaveN(writer);					
					writer.WriteLine(@"=");	
					writer.WriteLine(@"\{");	
					concat.EntityCollection[i].Grammar.SaveUnterminals(writer);
					writer.WriteLine(@"\}");	
					writer.WriteLine(@"\end{math}");						
					writer.WriteLine(@"--- множество нетерминальных символов грамматики",true);	
					writer.WriteLine(@"\begin{math}");
					concat.EntityCollection[i].Grammar.SaveG(writer);
					writer.WriteLine();	
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@";");	
					writer.WriteLine(@"\begin{math}");
					concat.EntityCollection[i].Grammar.SaveP(writer);					
					writer.WriteLine(@"=");	
					writer.WriteLine(@"\{");
					concat.EntityCollection[i].Grammar.SaveRules(writer);
					writer.WriteLine(@"\}");
					writer.WriteLine(@"\end{math}");
					writer.Write(@"--- множество правил вывода",true);
					if (concat.EntityCollection.Count - 1 == i)
						writer.WriteLine(@".",true);
					else
						writer.WriteLine(@";",true);
				}
				
				concat.GenerateGrammar(writer,isLeft,ref LastUseNumber, ref AddionalGrammarsNum);
				
				Grammar = concat.Grammar;
				
				writer.WriteLine(@"\end{enumerate}");
			}
		}
    }
}
