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

        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("mfenced", Writer.mathmlNS);
            writer.WriteAttributeString("open", "{");
            writer.WriteAttributeString("close", "}");
			
			writer.WriteStartElement("mrow", Writer.mathmlNS);

            writer.WriteStartElement("mfenced", Writer.mathmlNS);
			writer.WriteAttributeString("open", "");
            writer.WriteAttributeString("close", "");
			
            for (int i = 0; i < SetCollection.Count; i++)
			{
				writer.WriteStartElement("mrow", Writer.mathmlNS);				
                SetCollection[i].Save(writer);
				writer.WriteEndElement(); // mrow
			}

            writer.WriteEndElement(); // mfenced
			
			writer.WriteStartElement("mo", Writer.mathmlNS);
            writer.WriteAttributeString("stretchy", "true");
            writer.WriteString("|");
			writer.WriteEndElement(); // mo
			
            writer.WriteStartElement("mrow", Writer.mathmlNS);

            // 8704 - для всех
            writer.WriteStartElement("mo", Writer.mathmlNS);
            writer.WriteCharEntity((char)8704); 
            writer.WriteEndElement(); // mo

            writer.WriteStartElement("mfenced", Writer.mathmlNS);
            writer.WriteAttributeString("open", "");
            writer.WriteAttributeString("close", "");
            
            for (int i = 0; i < VariableCollection.Count; i++)
            {
                writer.WriteStartElement("mrow",Writer.mathmlNS);

                writer.WriteStartElement("mi", Writer.mathmlNS);
                writer.WriteCharEntity(VariableCollection[i].Name);
                writer.WriteEndElement(); // mi

                writer.WriteStartElement("mo", Writer.mathmlNS);

                if (VariableCollection[i].Sign == SignEnum.MoreThanZero)
                    writer.WriteCharEntity('>');
                else if (VariableCollection[i].Sign == SignEnum.MoreOrEqualZero)
                    writer.WriteCharEntity('≥');

                writer.WriteEndElement(); // mo

                writer.WriteStartElement("mn", Writer.mathmlNS);
                writer.WriteValue(VariableCollection[i].Num);
                writer.WriteEndElement(); // mn

                writer.WriteEndElement(); // mrow
            }

            writer.WriteEndElement(); // mfenced

            writer.WriteElementString("mtext", Writer.mathmlNS, ", где ");

            writer.WriteStartElement("mrow", Writer.mathmlNS);

            writer.WriteStartElement("mfenced", Writer.mathmlNS);
			writer.WriteAttributeString("open", "");
            writer.WriteAttributeString("close", "");

            for (int i = 0; i < VariableCollection.Count; i++)
            {
                writer.WriteStartElement("mi", Writer.mathmlNS);
                writer.WriteCharEntity(VariableCollection[i].Name);
                writer.WriteEndElement(); // mi
            }

            writer.WriteEndElement(); // mfenced

            writer.WriteElementString("mtext", Writer.mathmlNS, " " + (char)8212 + " целые");

            writer.WriteEndElement(); // mrow

            writer.WriteEndElement(); // mrow
			
			writer.WriteEndElement(); // mrow

            writer.WriteEndElement(); // mfenced
        }
    }
}
