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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("regular.log", restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Console()
                .CreateLogger();
            
            try
            {
                var client = new VmClient(configuration["VMindbookeUrl"]);
                var userToBoost = client.Register(new UserName("Stepan M"));

                var builder = new ContainerBuilder();
                builder.RegisterInstance(client).As<IVmClient>().SingleInstance();
                builder.RegisterType<CommentContentProvider>().As<ICommentContentProvider>().SingleInstance();
                builder.RegisterType<PostTitleProvider>().As<IPostTitleProvider>().SingleInstance();
                builder.Register(c => new UserCredentials(userToBoost.Id, userToBoost.Token)).As<UserCredentials>()
                    .SingleInstance();
                RegisterThresholds(builder, configuration);
                CreateBoostingJobs(builder);
                var container = builder.Build();
                
                GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(container));
                GlobalConfiguration.Configuration.UseMemoryStorage();

                var jobsContainer = container.Resolve<BoostingJobsContainer>();
                jobsContainer.StartJobs();
                
                using var backgroundJobServer = new BackgroundJobServer();
                Log.Information("Background service started");
                Console.ReadKey();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "The application has failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
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
                new PostLikesToCommentThreshold(configuration.GetValue<int>("PostLikesToComment")));
            builder.RegisterInstance(
                new CommentLikesToReplyThreshold(configuration.GetValue<int>("CommentLikesToReply")));
            builder.RegisterInstance(
                new PostLikesToStealThreshold(configuration.GetValue<int>("PostLikesToSteal")));
            builder.RegisterInstance(
                new UserLikesToStealPostThreshold(configuration.GetValue<int>("UserLikesToStealHisBestPost")));
            builder.RegisterInstance(
                new UserLikesThreshold(configuration.GetValue<int>("UserLikesLimit")));
        }
    }
}