using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Serilog;
using Serilog.Events;

namespace CarmaFucker
{
    static class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("regular.log", restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Console(
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "[{Timestamp:HH:mm:ss}][{Level:u4}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            GlobalConfiguration.Configuration.UseMemoryStorage();
            RecurringJob.AddOrUpdate(
                "JobController",
                () => StartOrStopJobs(),
                Cron.Minutely);
            using var backgroundServer = new BackgroundJobServer();
            Console.ReadKey();
        }

        public static void StartOrStopJobs()
        {
            Log.Information("Checking user likes...");
            if (new CarmaFucker().IsContinueCheating())
            {
                RecurringJob.AddOrUpdate<CarmaFucker>(
                    "CommentLikedPosts",
                    (client) => client.CommentLikedPosts(),
                    Cron.Minutely);
                RecurringJob.AddOrUpdate<CarmaFucker>(
                    "ReplyLikedComments",
                    (client) => client.ReplyLikedComments(),
                    Cron.Minutely);
                RecurringJob.AddOrUpdate<CarmaFucker>(
                    "CopyPostLikedPosts",
                    (client) => client.CopyPostLikedPosts(),
                    Cron.Minutely);
                RecurringJob.AddOrUpdate<CarmaFucker>(
                    "CopyPostLikedUserPost",
                    (client) => client.CopyPostLikedUserPost(),
                    Cron.Minutely);
                Log.Information("Cheating continues!");
            }
            else
            {
                RecurringJob.RemoveIfExists("CommentLikedPosts");
                RecurringJob.RemoveIfExists("ReplyLikedComments");
                RecurringJob.RemoveIfExists("CopyPostLikedPosts");
                RecurringJob.RemoveIfExists("CopyPostLikedUserPost");
                Log.Information("Stopping cheating for a while.");
            }
        }
    }
}