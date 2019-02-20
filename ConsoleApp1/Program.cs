namespace ConsoleApp1
{
    using System;

    using Serilog;

    class Program
    {
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Log.Logger = log;
            Log.Information("Hello, Serilog!");
            Console.ReadLine();
        }
    }
}
