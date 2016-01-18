using FLaG.IO.Input;
using FLaG.IO.Output;
using System;

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

			TaskDescription taskDescription = TaskDescription.Load(args[0]);
			taskDescription.Solve(args[1]);
		}
	}
}
