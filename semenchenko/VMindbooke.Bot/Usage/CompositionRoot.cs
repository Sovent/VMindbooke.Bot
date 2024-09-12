using Microsoft.Extensions.Configuration;
using RestSharp;
using Serilog;
using Serilog.Events;
using VMindbooke.Bot.App;
using VMindbooke.Bot.Infrastructure;

namespace Usage
{
    public class CompositionRoot
    {
        public IVMindbookeBotService Service { get; private set; }

        public static CompositionRoot Create(string config)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(config)
                .Build();
            var logger = new LoggerConfiguration()
                .WriteTo.File(configuration["LogFilePath"], LogEventLevel.Information)
                .CreateLogger();
            Log.Logger = logger;

            var restClient = new RestClient(configuration["VMindbookeURL"]);
            var repository = new PostsRepository(restClient);
            var factory = new NaiveShitPostFactory();
            var actionMaker = new ActionMaker(
                factory,
                restClient,
                configuration["VMindbookeToken"],
                int.Parse(configuration["VMindbookeId"]));

            return new CompositionRoot
            {
                Service = new VMindbookeBotService(
                    configuration,
                    actionMaker,
                    repository,
                    restClient)
            };
        }
    }
}