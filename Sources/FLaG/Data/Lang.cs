using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using FLaG.Output;

namespace FLaG.Data
{
    class Lang
    {
		public int? NumLabel
		{
			get;
			set;
		}
		
        public Lang()
        {
            VariableCollection = new List<Variable>();
            SetCollection = new List<Entity>();
        }

        public Lang(string filename)
            : this()
        {
            Load(filename);
        }

        public Lang(Stream stream)
            : this()
        {
            Load(stream);
        }

        private void Load(XmlReader reader)
        {
            while (!reader.IsStartElement("lang"))
                reader.Read();

            reader.ReadStartElement(); // lang

            while (!reader.IsStartElement("variables"))
                reader.Read();

            bool isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement(); // variables

            while (reader.IsStartElement("variable"))                
            {
                Variable var = new Variable(reader);
                VariableCollection.Insert(~VariableCollection.BinarySearch(var),var);
            }

            if (!isEmpty)
                reader.ReadEndElement(); // variables

            isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement(); // sets

            while (reader.IsStartElement("set"))
            {
                reader.ReadStartElement(); // set

                if (reader.IsStartElement())
                    SetCollection.Add(Entity.Load(reader,VariableCollection));

                reader.ReadEndElement(); // set
            }

            if (!isEmpty)
                reader.ReadEndElement(); // sets

            reader.ReadEndElement(); // lang
        }

        private void Load(Stream stream)
        {
            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            Stream schema = assembly.GetManifestResourceStream("FLaG.Schema.lang.xsd");

            XmlSchema xmlSchema = XmlSchema.Read(schema, null);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags =
                System.Xml.Schema.XmlSchemaValidationFlags.ReportValidationWarnings |
                System.Xml.Schema.XmlSchemaValidationFlags.ProcessIdentityConstraints;
            settings.IgnoreComments = true;
            settings.ValidationEventHandler += new ValidationEventHandler(settings_ValidationEventHandler);
            settings.Schemas.Add(xmlSchema);

            using (XmlReader reader = XmlReader.Create(stream, settings))
                Load(reader);
        }

        private void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }

        private void Load(string filename)
        {
            using (FileStream fileStream =
                new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                Load(fileStream);
        }
        
        public List<Variable> VariableCollection
        {
            get;
            private set;
        }

        public List<Entity> SetCollection
        {
            get;
            private set;
        }

        public Lang ToRegularSet()
        {
            Lang l = new Lang();
            l.VariableCollection.AddRange(VariableCollection);

            foreach (Entity e in SetCollection)
                l.SetCollection.Add(e.ToRegularSet());

            return l;
        }
		
		public Lang ToRegularExp()
		{
			Lang l = new Lang();		
			
			foreach (Entity e in SetCollection)
				l.SetCollection.Add(e.ToRegularExp());
			
			return l;
		}
		
		public List<Entity> MarkDeepest()
		{
			List<Entity> list = new List<Entity>();
			
			if (NumLabel != null)
				return list;
			
			int v = 1,oldv;

			do
			{
				oldv = v;
				
				foreach (Entity e in SetCollection)
					v = e.MarkDeepest(v, list);
				
			} while (oldv != v);
						
			NumLabel = v;
			
			Alter alter = new Alter();
			
			alter.NumLabel = NumLabel;
			
			foreach (Entity e in SetCollection)
				alter.EntityCollection.Add(e);
			
			list.Add(alter);
			
			return list;
		}
		
		public Symbol[] CollectAlphabet()
		{
			List<Symbol> symbols = new List<Symbol>();
			
			foreach (Entity e in SetCollection)
			{
				Symbol[] smbs = e.CollectAlphabet();
				foreach (Symbol s in smbs)
				{
					int index = symbols.BinarySearch(s);
					
					if (index < 0)
						symbols.Insert(~index,s);
				}
			}
			
			return symbols.ToArray();
		}
		
		public void SaveAsRegularExp(Writer writer, bool full)
		{
			if (full)
				writer.Write(@"{\underbrace");
			
			writer.Write("{");
			
			for (int i = 0; i < SetCollection.Count; i++)
			{
				if (i != 0)
					writer.Write('+');
                SetCollection[i].SaveAsRegularExp(writer, full);
			}
			
			writer.Write("}");
			
			if (full)
			{
				writer.Write(@"_\text{");
				writer.Write(NumLabel);
				writer.Write(@"}}");
			}
		}
		
		public void SaveAlphabet(Writer writer)
		{
			Symbol[] symbols = CollectAlphabet();
			
			writer.Write(@"\Sigma=");
			
			writer.Write("{",true);
			
			for (int i = 0; i < symbols.Length; i++)			
			{
				if (i != 0)
					writer.Write(",",true);
				
				symbols[i].Save(writer);
			}
				
			writer.Write("}",true);
		}

        public void Save(Writer writer)
        {
			writer.Write(@"\left\{");
			
            for (int i = 0; i < SetCollection.Count; i++)
			{
				if (i != 0)
					writer.Write(',');
                SetCollection[i].Save(writer);
			}
			
			writer.Write(@"\mid ");
			
			writer.Write(@" \forall ");
			
            for (int i = 0; i < VariableCollection.Count; i++)
            {
				if (i != 0)
					writer.Write(',');
				
				writer.Write(VariableCollection[i].Name);

                if (VariableCollection[i].Sign == SignEnum.MoreThanZero)
                    writer.Write('>');
                else if (VariableCollection[i].Sign == SignEnum.MoreOrEqualZero)
                    writer.Write(@" \geq ");
				
				writer.Write(VariableCollection[i].Num);
            }
			
			writer.Write(@", \text{где } ");
			
            for (int i = 0; i < VariableCollection.Count; i++)
            {
				if (i != 0)
					writer.Write(',');
				
				writer.Write(VariableCollection[i].Name);
            }
			
			writer.Write(@"  \text{ --- целые}");
			
			writer.Write(@"\right\}");
        }
    }
}
