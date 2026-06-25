using FLaG.IO.In;
using FLaG.IO.Out;

namespace FLaG
{
    static class Program
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
