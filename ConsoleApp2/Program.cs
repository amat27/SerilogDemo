namespace ConsoleApp2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Serilog;

    class Program
    {
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration().Enrich.WithProperty("Source", "Program").WriteTo.ColoredConsole(
                    outputTemplate:"[{Timestamp:HH:mm:ss} {Level:u3} {Source}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            log.Information("Start simulating");
            OverCooking.Simulate();
            log.Information("Simulating asynchronously");
            Console.ReadLine();
        }
    }
}