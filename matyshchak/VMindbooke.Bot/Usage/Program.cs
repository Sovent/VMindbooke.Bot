using System;
using Autofac;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using VMindbookeBooster;
using VMindbookeBooster.Entities;
using VMindbookeClient;

namespace Usage
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            
            var builder = new ContainerBuilder();
            builder.Register(c => new VmClient(configuration["VMindbookeUrl"])).As<IVmClient>();
            builder.Register(c => new VmBooster(c.Resolve<IVmClient>())).As<VmBooster>().SingleInstance();
            var container = builder.Build();
            GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(container));
            
            var logger = new LoggerConfiguration()
                .WriteTo.File("regular.log", restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Console()
                .CreateLogger();
            
            GlobalConfiguration.Configuration.UseMemoryStorage();

            var client = container.Resolve<IVmClient>();
            var user = client.Register(new UserName("STEPA"));

            RecurringJob.AddOrUpdate<VmBooster>((booster) => booster.CommentPostsWithLikes(user.Id,
                    user.Token,
                    configuration.GetValue<int>("MinDailyLikesToCommentPost"),
                    new CommentContent("COMMENT")),
                Cron.Minutely);
            
            RecurringJob.AddOrUpdate<VmBooster>((booster) => booster.ReplyToCommentWithLikes(user.Id,
                    user.Token,
                    configuration.GetValue<int>("MinDailyLikesToReplyComment"),
                    new CommentContent("COMMENT")),
                Cron.Minutely);
            
            RecurringJob.AddOrUpdate<VmBooster>((booster) => booster.StealPostWithLikes(user.Id,
                    user.Token,
                    configuration.GetValue<int>("MinDailyLikesToStealPost"),
                    "STOLEN POST"),
                Cron.Minutely);
            
            RecurringJob.AddOrUpdate<VmBooster>((booster) => booster.StealTheBestPostOfMostLikedUser(user.Id,
                    user.Token,
                    configuration.GetValue<int>("MinDailyUserLikesToStealHisBestPost")),
                Cron.Minutely);
            
            RecurringJob.AddOrUpdate<VmBooster>((booster) => booster.StealTheBestPostOfMostLikedUser(user.Id,
                    user.Token,
                    configuration.GetValue<int>("MinDailyUserLikesToStealHisBestPost")),
                Cron.Minutely);
            
            using (var backgroundServer = new BackgroundJobServer())
            {
                logger.Information("Background service started");
                Console.ReadKey();
            }
        }

        public class ContainerJobActivator : JobActivator
        {
            private IContainer _container;

            public ContainerJobActivator(IContainer container)
            {
                _container = container;
            }

            public override object ActivateJob(Type type)
            {
                return _container.Resolve(type);
            }
        }
    }
}