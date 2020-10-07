using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using VMindBooke.SDK.Application;
using VMindBooke.SDK.Domain;

namespace CarmaFucker
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("regular.log", restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("CarmaFucker started");
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var vMindBookeClient = VMindBookeFactory.VMindBookeClientBuild(configuration["serverAddress"]);
            var mainUser = vMindBookeClient.UserService.GetAuthorizedUser(
                configuration.GetValue<int>("userId"),
                configuration.GetValue<string>("userToken"));
            var carmaFucker = new CarmaFucker(
                vMindBookeClient,
                mainUser,
                configuration.GetValue<int>("postLikesToComment"),
                configuration.GetValue<int>("commentLikesToReply"),
                configuration.GetValue<int>("postLikesToCopyPost"),
                configuration.GetValue<int>("userLikesToCopyPost"),
                configuration.GetValue<int>("selfLikesToStop")
                );
            
            var likesCountLastDay = mainUser.LikesList.Where(like => (DateTime.Now - like.PlacingDateUtc).Days == 0).Count();
            RecurringJob.AddOrUpdate(
                "CommentLikedPosts",
                () => carmaFucker.CommentLikedPosts(configuration.GetValue<int>("postLikesToComment")),
                Cron.Minutely);
            RecurringJob.AddOrUpdate(
                "ReplyLikedComments",
                () => carmaFucker.ReplyLikedComments(configuration.GetValue<int>("commentLikesToReply")),
                Cron.Minutely);
            RecurringJob.AddOrUpdate(
                "CopyPostLikedPosts",
                () => carmaFucker.CopyPostLikedPosts(configuration.GetValue<int>("postLikesToCopyPost")),
                Cron.Minutely);
            RecurringJob.AddOrUpdate(
                "CopyPostLikedUserPost",
                () => carmaFucker.CopyPostLikedUserPost(configuration.GetValue<int>("userLikesToCopyPost")),
                Cron.Minutely);
            using (var backgroundServer = new BackgroundJobServer())
            {
                if (carmaFucker.IsContinueCheating(configuration.GetValue<int>("selfLikesToStop")))
                {
                    
                    RecurringJob.AddOrUpdate(
                        "CommentLikedPosts",
                        () => carmaFucker.CommentLikedPosts(configuration.GetValue<int>("postLikesToComment")),
                        Cron.Minutely);
                    RecurringJob.AddOrUpdate(
                        "ReplyLikedComments",
                        () => carmaFucker.ReplyLikedComments(configuration.GetValue<int>("commentLikesToReply")),
                        Cron.Minutely);
                    RecurringJob.AddOrUpdate(
                        "CopyPostLikedPosts",
                        () => carmaFucker.CopyPostLikedPosts(configuration.GetValue<int>("postLikesToCopyPost")),
                        Cron.Minutely);
                    RecurringJob.AddOrUpdate(
                        "CopyPostLikedUserPost",
                        () => carmaFucker.CopyPostLikedUserPost(configuration.GetValue<int>("userLikesToCopyPost")),
                        Cron.Minutely);
                }
                else
                {
                    RecurringJob.RemoveIfExists("CommentLikedPosts");
                    RecurringJob.RemoveIfExists("ReplyLikedComments");
                    RecurringJob.RemoveIfExists("CopyPostLikedPosts");
                    RecurringJob.RemoveIfExists("CopyPostLikedUserPost");
                }

                Console.ReadKey();
            }
        }

        
    }
}