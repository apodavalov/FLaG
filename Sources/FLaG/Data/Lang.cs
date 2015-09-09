using FLaG.Output;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace FLaG.Data
{
    class Lang
    {
		public string Variant
		{
			get;
			set;
		}
		
		public string Group
		{
			get;
			set;
		}
		
		public string FirstName
		{
			get;
			set;
		}
		
		public string SecondName
		{
			get;
			set;
		}
		
		public string LastName			
		{
			get;
			set;
		}
		
		public Entity Content
		{
			get;
			set;
		}
				
        public Lang()
        {
            VariableCollection = new List<Variable>();
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

            reader.ReadStartElement(); // content

            if (reader.IsStartElement())
            	Content = Entity.Load(reader,VariableCollection);

            if (!isEmpty)
                reader.ReadEndElement(); // content
			
			reader.ReadStartElement("info");
			
			while (reader.IsStartElement())
			{
				if (reader.Name == "author")
				{
					isEmpty = reader.IsEmptyElement;
					
					if (reader.HasAttributes)
						while (reader.MoveToNextAttribute())
							switch (reader.Name)
							{
								case "firstname":
									FirstName = reader.Value;
									break;
								case "secondname":
									SecondName = reader.Value;
									break;								
								case "lastname":
									LastName = reader.Value;
									break;																
								case "group":
									Group = reader.Value;
									break;
							}
				
					reader.ReadStartElement("author");
					
					if (!isEmpty)
                		reader.ReadEndElement(); // author
				}
				else if (reader.Name == "variant")
				{
					isEmpty = reader.IsEmptyElement;	
					
					reader.ReadStartElement("variant");
					Variant = reader.ReadContentAsString();
					
					if (!isEmpty)
                		reader.ReadEndElement(); // variant
				}
			}
			
			reader.ReadEndElement(); // info

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

        public Lang ToRegularSet()
        {
            Lang l = new Lang();
            l.VariableCollection.AddRange(VariableCollection);
			
			l.Content = Content.ToRegularSet();

            return l;
        }
		
		public Lang ToRegularExp()
		{
			Lang l = new Lang();		
			
			l.Content = Content.ToRegularExp();
			
			return l;
		}
		
		public List<Entity> MarkDeepest()
		{
			List<Entity> list = new List<Entity>();
			
			int v = 1, oldv;
			
			do
			{
				oldv = v;
				v = Content.MarkDeepest(v, list);
			} while (oldv != v);
			
			return list;
		}
		
		public Symbol[] CollectAlphabet()
		{
			return Content.CollectAlphabet();
		}
		
		public void SaveAsRegularExp(Writer writer, bool full)
		{
			Content.SaveAsRegularExp(writer,full);
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
			
            Content.Save(writer);
			
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
