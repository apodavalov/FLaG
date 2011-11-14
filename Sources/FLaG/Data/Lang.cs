﻿using System;
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
		
		public void SaveAsRegularExp(Writer writer)
		{
			for (int i = 0; i < SetCollection.Count; i++)
			{
				if (i != 0)
					writer.Write('+');
                SetCollection[i].SaveAsRegularExp(writer);
			}
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
			
			writer.Write("|");
			
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
