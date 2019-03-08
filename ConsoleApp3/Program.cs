namespace ConsoleApp3
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Serilog;

    class Program
    {
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();

            Serilog.Debugging.SelfLog.Enable(Console.Out);
            Log.Logger = log;
            Trace.TraceInformation("Tracing test");
            Console.ReadLine();
            Log.CloseAndFlush();
        }
    }
}
