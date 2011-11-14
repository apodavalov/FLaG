using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
			
			using (Writer writer = new Writer(args[1],false,new UTF8Encoding(false)))
            {
                writer.WriteStartDoc();
                writer.Step1();
				writer.Step2();
				Lang regular = lang.ToRegularSet();
                writer.Step2_1(lang, regular);
				writer.Step2_2(regular.ToRegularExp());
                writer.WriteEndDoc();
            }
        }
    }
}
