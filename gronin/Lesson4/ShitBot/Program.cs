﻿using System;
using Serilog;
using Serilog.Events;


namespace ShitBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
               .WriteTo.File("reg.log", restrictedToMinimumLevel: LogEventLevel.Information)
               .WriteTo.Console(LogEventLevel.Verbose)
               .CreateLogger();
           
           Log.Logger = logger;
           Log.Logger.Information("Check");
           Console.WriteLine("csccsc");
           var service = new ShitBotService();
           service.StartFarming();

        }
    }
}
