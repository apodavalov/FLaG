using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	class Grammar
	{
		public int Number
		{
			get;
			set;
		}
		
		public bool IsLeft
		{
			get;
			set;
		}
		
		public string Apostrophs
		{
			get
			{
				if (IsLeft)
					return "'";
				else
					return "''";
			}
		}
		
		public List<Rule> Rules
		{
			get;
			private set;
		}
		
		public Unterminal[] Unterminals
		{
			get
			{
				Unterminal[] unterminals = new Unterminal[Rules.Count];
				
				for (int i = 0; i < Rules.Count; i++)					
					unterminals[i] = Rules[i].Prerequisite;
				
				return unterminals;
			}
		}
		
		public void SaveUnterminals(Writer writer)
		{
			Unterminal[] unterminals = Unterminals;
			
			for (int i = 0; i < unterminals.Length; i++)
			{
				if (i != 0)		
					writer.Write(",");
				
				unterminals[i].Save(writer, IsLeft);
			}
		}
		
		public void SaveRules(Writer writer)
		{
			for (int i = 0; i < Rules.Count; i++)
			{
				if (i != 0)		
					writer.Write(",");
				
				Rules[i].Save(writer, IsLeft);
			}
		}
		
		private void SaveLetter(char Letter, Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{");
			writer.Write(Letter.ToString(), true);
			writer.Write("_");
			writer.Write(Number);
			writer.Write(@"}");
			writer.Write(Apostrophs);
			writer.Write(@"}");
		}
		
		public void SaveCortege(Writer writer)
		{
			SaveLetter('G',writer);
			writer.Write(@"=");
			writer.Write(@"\left(");			
			SaveN(writer);
			writer.Write(@",");
			writer.Write(@"\Sigma ");
			writer.Write(@",");			
			SaveP(writer);						
			writer.Write(@",");
			SaveS(writer);						
			writer.Write(@"\right)");
		}
		
		public void SaveN(Writer writer)
		{
			SaveLetter('N',writer);
		}
		
		public void SaveP(Writer writer)
		{
			SaveLetter('P',writer);
		}
		
		public void SaveS(Writer writer)
		{
			SaveLetter('S',writer);
		}
		
		public void SaveG(Writer writer)
		{
			SaveLetter('G',writer);
		}
		
		public Grammar ()
		{
			Rules = new List<Rule>();
		}
	}
}

