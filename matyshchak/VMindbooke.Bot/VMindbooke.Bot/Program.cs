using System;
using Autofac;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Usage.Domain;
using Usage.Domain.ContentProviders;
using Usage.Domain.Entities;
using Usage.Domain.Jobs;
using Usage.Domain.ValueObjects;
using Usage.Infrastructure;
using PostCommentingJob = Usage.Domain.Jobs.PostCommentingJob;

namespace Usage
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var client = new VmClient(configuration["VMindbookeUrl"]);
            
            var user = client.Register(new UserName("STEPA"));
            
            var builder = new ContainerBuilder();
            builder.RegisterInstance(client).As<IVmClient>().SingleInstance();
            builder.RegisterType<CommentContentProvider>().As<ICommentContentProvider>().SingleInstance();
            builder.RegisterType<PostTitleProvider>().As<IPostTitleProvider>().SingleInstance();
            builder.Register(c => new UserCredentials(user.Id, user.Token)).As<UserCredentials>().SingleInstance();
            RegisterThresholds(builder, configuration);
            CreateBoostingJobs(builder);
            var container = builder.Build();
            
            GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(container));
            GlobalConfiguration.Configuration.UseMemoryStorage();

            var logger = new LoggerConfiguration()
                .WriteTo.File("regular.log", restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Console()
                .CreateLogger();

            var jobsContainer = container.Resolve<BoostingJobsContainer>();
            jobsContainer.StartJobs();

            using var backgroundJobServer = new BackgroundJobServer();
            logger.Information("Background service started");
            
            Console.ReadKey();
        }

        private static void CreateBoostingJobs(ContainerBuilder container)
        {
            var jobsContainer = new BoostingJobsContainer();
            void CreateJob<T>() where T : IBoostingJob
            {
                jobsContainer.Add<T>();
                container.RegisterType<T>().SingleInstance();
            }
            
            CreateJob<PostCommentingJob>();
            CreateJob<CommentReplyingJob>();
            CreateJob<PostStealingJob>();
            CreateJob<LikeLimitCheckingJob>();

            container.RegisterInstance(jobsContainer).SingleInstance();
        }

        private static void RegisterThresholds(ContainerBuilder builder, IConfigurationRoot configuration)
        {
            builder.RegisterInstance(
                new PostLikesToCommentThreshold(configuration.GetValue<int>("MinDailyPostLikesToComment")));
            builder.RegisterInstance(
                new CommentLikesToReplyThreshold(configuration.GetValue<int>("MinDailyLikesToReplyComment")));
            builder.RegisterInstance(
                new PostLikesToStealThreshold(configuration.GetValue<int>("MinDailyPostLikesToSteal")));
            builder.RegisterInstance(
                new UserLikesToStealPostThreshold(configuration.GetValue<int>("MinDailyUserLikesToStealHisBestPost")));
            builder.RegisterInstance(
                new UserLikesToStealPostThreshold(configuration.GetValue<int>("DailyBoostedUserLikesLimit")));
        }
    }
}