using System;

namespace KASM
{
    public static class Error
    {
        public static void ThrowError(string msg)
        {
            Console.Error.WriteLine("Error: " + msg);
            Environment.Exit(1);
        }
    }
}
