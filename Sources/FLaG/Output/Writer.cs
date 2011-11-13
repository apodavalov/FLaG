using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Data;

namespace FLaG.Output
{
    class Writer : IDisposable
    {
        public static string docBookNS = "http://docbook.org/ns/docbook";
        public static string xmlNS = "http://www.w3.org/XML/1998/namespace";
        public static string mathmlNS = "http://www.w3.org/1998/Math/MathML";

        public XmlWriter OutputWriter
        {
            get;
            private set;
        }

        public Writer(XmlWriter writer)
        {
            OutputWriter = writer;
        }

        public void WriteStartDoc()
        {
            OutputWriter.WriteStartDocument();

            OutputWriter.WriteStartElement("db", "article", docBookNS);

            OutputWriter.WriteStartAttribute("version");
            OutputWriter.WriteString("5.0");
            OutputWriter.WriteEndAttribute();

            OutputWriter.WriteStartAttribute("xml", "lang", xmlNS);
            OutputWriter.WriteString("ru");
            OutputWriter.WriteEndAttribute();

            OutputWriter.WriteStartElement("articleinfo", docBookNS);

            OutputWriter.WriteElementString("title", docBookNS, "Домашняя работа по ТЯП");

            OutputWriter.WriteStartElement("author", docBookNS);
            OutputWriter.WriteElementString("firstname", docBookNS, "Ленуза"); // TODO: имя из примера
            OutputWriter.WriteElementString("surname", docBookNS, "Подавалова"); // TODO: фамилия из примера
            OutputWriter.WriteEndElement(); // author

            OutputWriter.WriteEndElement(); // articleinfo
        }

        public void Step1()
        {
            // TODO: реализовать шаг 1
        }

        public void Step2_1(Lang inputLang, Lang outputLang)
        {
            OutputWriter.WriteStartElement("section", docBookNS);
            OutputWriter.WriteAttributeString("id", xmlNS, "step-2-1");

            OutputWriter.WriteElementString("title", docBookNS, "Шаг 2.1");
            OutputWriter.WriteStartElement("para", docBookNS);
            OutputWriter.WriteString("Перед построением регулярной грамматики для регулярного ");
            OutputWriter.WriteString("языка требуется, чтобы данный язык был определен с помощью ");
            OutputWriter.WriteString("регулярного выражения, которое в свою очередь представляет ");
            OutputWriter.WriteString("регулярное множество. Чтобы множество, определяющее исходный язык ");
            OutputWriter.WriteElementString("emphasis", docBookNS, "L");
            OutputWriter.WriteString(", было регулярным необходимо сделать следующие эквивалентные ");
            OutputWriter.WriteString("преобразования:");
            OutputWriter.WriteEndElement(); // para

            OutputWriter.WriteStartElement("math","math",mathmlNS);
			OutputWriter.WriteAttributeString("linebreakstyle","after");

            OutputWriter.WriteStartElement("mrow",mathmlNS);
            OutputWriter.WriteElementString("mi",mathmlNS,"L");
            OutputWriter.WriteElementString("mo", mathmlNS, "=");
            inputLang.Save(OutputWriter);
			OutputWriter.WriteStartElement("mo", mathmlNS);
			OutputWriter.WriteAttributeString("linebreak","newline");
			OutputWriter.WriteString("=");
			OutputWriter.WriteEndElement();
            outputLang.Save(OutputWriter);
            OutputWriter.WriteEndElement(); // mrow

            OutputWriter.WriteEndElement(); // math

            OutputWriter.WriteEndElement(); // section
        }

        public void WriteEndDoc()
        {
            OutputWriter.WriteEndElement(); // article
            OutputWriter.WriteEndDocument();
        }

        public void Dispose()
        {
            if (OutputWriter != null)
                OutputWriter.Close();
        }
    }
}
