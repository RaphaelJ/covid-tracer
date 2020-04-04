using System;

namespace CovidTracer
{
    public static class Logger
    {
        public static void Info(string message)
        {
            Console.WriteLine("[CovidTracer] " + message);
        }
    }
}
