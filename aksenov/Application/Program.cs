using System;
using VMindbooke.Bot;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var settings = BotSettings.FromJsonFile("appsettings.json");

            Console.WriteLine();
        }
    }
}