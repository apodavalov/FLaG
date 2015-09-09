using FLaG.Data;
using FLaG.Output;
using System;
using System.IO;
using System.Text;

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
			
			FileInfo fileInfo = new FileInfo(args[1]);
			
			string outputFileNamePrefix = 
				fileInfo.FullName.Substring(0,fileInfo.FullName.Length - fileInfo.Extension.Length) + ".";
			
			outputFileNamePrefix = outputFileNamePrefix.Replace('.','-');
			
			using (Writer writer = new Writer(args[1],false,new UTF8Encoding(false),lang))
			{
				writer.OutputFileNamePrefix = outputFileNamePrefix;
				writer.Out();
			}
        }
    }
}
