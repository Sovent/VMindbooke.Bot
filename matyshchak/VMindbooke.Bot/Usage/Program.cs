using System;
using System.Collections.Generic;
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
        private static Action _boosterJobs = () => { };
        private static HashSet<string> _boosterJobsIds = new HashSet<string>();

        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var client = new VmClient(configuration["VMindbookeUrl"]);
            var user = client.Register(new UserName("STEPA"));
            var builder = new ContainerBuilder();
            builder.Register(c => new VmClient(configuration["VMindbookeUrl"])).As<IVmClient>().SingleInstance();
            builder.RegisterType<LikeLimitChecker>().As<ILikeLimitChecker>().SingleInstance();
            builder.RegisterType<PostCommenter>().As<IPostCommenter>().SingleInstance();
            builder.RegisterType<CommentReplier>().As<ICommentReplier>().SingleInstance();
            builder.RegisterType<PostsStealer>().As<IPostsStealer>().SingleInstance();
            builder.Register(c => new UserCredentials(user.Id, user.Token)).As<UserCredentials>().SingleInstance();
            builder.RegisterType<VmBoosterBot>().WithParameter("likeLimit", configuration.GetValue<int>("DailyLikesLimit"));
            var container = builder.Build();
            
            GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(container));
            GlobalConfiguration.Configuration.UseMemoryStorage();

            var logger = new LoggerConfiguration()
                .WriteTo.File("regular.log", restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Console()
                .CreateLogger();

            var bbot = container.Resolve<VmBoosterBot>();
            bbot.CommentPosts(configuration.GetValue<int>("MinDailyLikesToCommentPost"), new CommentContent("COMMENT"))
                .ReplyComments(configuration.GetValue<int>("MinDailyLikesToReplyComment"), new CommentContent("REPLY"))
                .StartBoosting();


            logger.Information("Background service started");
            Console.ReadKey();
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