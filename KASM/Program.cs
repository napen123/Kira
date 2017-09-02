using System;
using System.IO;

namespace KASM
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage : KASM [input file] [output file]");

                return;
            }

            var input = args[0];

            if (!File.Exists(input))
                Error.ThrowError("Could not find input file: " + input);
            
            Compiler.Compile(input, args[1]);
        }
    }
}
