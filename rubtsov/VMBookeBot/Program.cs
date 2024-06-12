using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.MemoryStorage;
using Serilog;
using VMBookeBot;
using VMBookeBot.Infrastructure;

namespace VMBookeBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var compositionRoot = new CompositionRoot();
            var jobClientService = compositionRoot.BotThiefService;
            var currentUserId = compositionRoot.CurrentUserId; 
            
            GlobalConfiguration.Configuration.UseMemoryStorage();
            RecurringJob.AddOrUpdate<BackgroundServiceProvider>(
                clientService=>clientService.WriteCommentService(), Cron.Minutely);
            RecurringJob.AddOrUpdate<BackgroundServiceProvider>(
                clientService=>clientService.WriteReplyService(), Cron.Minutely);
            RecurringJob.AddOrUpdate<BackgroundServiceProvider>(
                clientService=>clientService.StealPopularPostService(), Cron.Minutely);
            RecurringJob.AddOrUpdate<BackgroundServiceProvider>(
                clientService=>clientService.StealPostFromPopularUserService(), Cron.Minutely);
            
            var checkUserLikesAsync =  new Task(async delegate
            {
                while (!jobClientService.CurrentUserLikesLimitCheckService(currentUserId))
                {
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    Log.Debug("Checking user likes number");
                }
            });
            
            using (var backgroundServer = new BackgroundJobServer())
            {
                Log.Information("Background service started");
                checkUserLikesAsync.Start();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Log.Information("Background service stopped");
            }
        }
    }
}