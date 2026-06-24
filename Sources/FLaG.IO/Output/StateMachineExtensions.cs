using System.Text;
using System.Xml;
using FLaG.Core.Data.StateMachines;

namespace FLaG.IO.Output
{
    public static class StateMachineExtensions
    {
        public static void DrawDiagram(this StateMachine stateMachine, string fileName)
        {
            using FileStream fileStream = new(fileName,FileMode.Create, FileAccess.Write, FileShare.None);
            XmlWriterSettings settings = new()
            {
                CloseOutput = true,
                CheckCharacters = true,
                Encoding = new UTF8Encoding(false),
                Indent = true,

            };
            using XmlWriter xmlWriter = XmlWriter.Create(fileStream, settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("svg", "http://www.w3.org/2000/svg");
            xmlWriter.WriteAttributeString("width","1000");
            xmlWriter.WriteAttributeString("height","1000");
            xmlWriter.WriteAttributeString("viewBox", "0 0 100 100");
            xmlWriter.WriteStartElement("text");
            xmlWriter.WriteAttributeString("x","50");
            xmlWriter.WriteAttributeString("y","50");
            xmlWriter.WriteAttributeString("text-anchor","middle");
            xmlWriter.WriteAttributeString("dominant-baseline","middle");
            xmlWriter.WriteAttributeString("stroke-width","1");
            xmlWriter.WriteString("Hello, world!");
            xmlWriter.WriteEndElement(); // text
            xmlWriter.WriteEndElement(); // svg
            xmlWriter.WriteEndDocument();
        }
    }
}
