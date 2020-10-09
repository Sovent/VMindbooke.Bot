using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Serilog;

namespace Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.File("VMindbookeBoostingLog.log")
                .WriteTo.Console()
                .CreateLogger();
            Log.Logger = logger;

            GlobalConfiguration.Configuration.UseMemoryStorage();
            
            RecurringJob.AddOrUpdate<LikeBoostingClient>(
                "CommentsWriting", 
                client => client.CommentsWriting(), 
                "*/15 * * * *");
            RecurringJob.AddOrUpdate<LikeBoostingClient>(
                "RepliesWriting", 
                client => client.RepliesWriting(), 
                "*/15 * * * *");
            RecurringJob.AddOrUpdate<LikeBoostingClient>(
                "PostsCopyingByLikes", 
                client => client.PostsCopyingByLikes(), 
                "*/15 * * * *");
            RecurringJob.AddOrUpdate<LikeBoostingClient>(
                "PostsCopyingByUsers", 
                client => client.PostsCopyingByUsers(), 
                "*/15 * * * *");
            RecurringJob.AddOrUpdate<LikeBoostingClient>(
                "BoostFinish", 
                client => client.BoostFinish(), 
                "*/2 * * * *");

            using (var backgroundServer = new BackgroundJobServer())
            {
                Log.Information("Like boosting started.");
                RecurringJob.Trigger("CommentsWriting");
                RecurringJob.Trigger("RepliesWriting");
                RecurringJob.Trigger("PostsCopyingByLikes");
                RecurringJob.Trigger("PostsCopyingByUsers");
                RecurringJob.Trigger("BoostFinish");
                Console.ReadKey();
                Log.Information("Like boosting finished.");
            }
        }
    }
}