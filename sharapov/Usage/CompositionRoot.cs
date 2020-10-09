using System.Reflection;
using Serilog;
using Serilog.Events;
using VMindbookeBot;

namespace Usage
{
    public class CompositionRoot
    {
        public BotService BotService { get; private set; }

        public static CompositionRoot Create(string configFileName = null)
        {
            var configurationFile = new Configuration(configFileName ?? "appsettings.json");
            return new CompositionRoot
            {
                BotService = BotService.Create(configurationFile)
            };
        }
    }
}