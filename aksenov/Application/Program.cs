using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Serilog;
using VMindbooke.Bot;
using VMindbooke.Bot.Application;
using VMindbooke.Bot.Infrastructure;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = BotSettings.FromJsonFile("appsettings.json");
            
            var logger = new LoggerConfiguration()
                .WriteTo.File("regular.log")
                .WriteTo.Console()
                .CreateLogger();

            var apiRequestsService = new APIRequestsService(settings, logger);
            
            var service = new LikeBoostingService(settings, 
                apiRequestsService, 
                DateTime.Now,
                new SpamRepository(), 
                new HashesRepository(), 
                new ProcessedObjectsRepository(), 
                logger);

            GlobalConfiguration.Configuration.UseMemoryStorage();
            RecurringJob.AddOrUpdate(() => service.BoostFinishScenario(), "*/5 * * * *");
            RecurringJob.AddOrUpdate(() => service.CommentsWritingScenario(), "*/10 * * * *");
            RecurringJob.AddOrUpdate(() => service.RepliesWritingScenario(), "*/10 * * * *");
            RecurringJob.AddOrUpdate(() => service.PostsCopyingByLikesScenario(), "*/10 * * * *");
            RecurringJob.AddOrUpdate(() => service.PostsCopyingByUsersScenario(), "*/10 * * * *");

            using (var backgroundServer = new BackgroundJobServer())
            {
                logger.Information("Like boosting started.");
                Console.WriteLine("Press any key to finish boosting.");
                Console.ReadKey();
            }
        }
    }
}