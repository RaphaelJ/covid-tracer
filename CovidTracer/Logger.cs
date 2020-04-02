using System;
namespace CovidTracer
{
    public static class Logger
    {
        public static void write(string message)
        {
            Console.WriteLine("[CovidTracer] " + message);
        }
    }
}
