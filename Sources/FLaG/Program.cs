using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLaG.Data;
using System.Xml;
using FLaG.Output;

namespace FLaG
{
    class Program
    {
        static void Main(string[] args)
        {
			if (args.Length < 2)
			{
				Console.WriteLine("Usage:");
				Console.WriteLine("\tFLaG.exe <input> <output>");
				return;
			}
			
            Lang lang = new Lang(args[0]);

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.IndentChars = "\t";
            settings.Indent = true;
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
            
            using (Writer writer = new Writer(XmlWriter.Create(args[1], settings)))
            {
                writer.WriteStartDoc();
                writer.Step1();
                writer.Step2_1(lang,lang.ToRegularSet());
                writer.WriteEndDoc();
            }
        }
    }
}
