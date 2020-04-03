using System;
namespace CovidTracer
{
    public static class Logger
    {
        public static void Write(string message)
        {
            Console.WriteLine("[CovidTracer] " + message);
        }
    }
}
