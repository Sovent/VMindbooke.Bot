using System;
using Hangfire;
using Hangfire.MemoryStorage;

namespace VMindbooster
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup logger here
            
            // imagine this is from configuration
            var thresholdConfiguration = new ThresholdConfiguration(likedPostThreshold: 15, likedCommentThreshold: 20);

            var container = CheatingJobContainer.Setup(thresholdConfiguration);

            GlobalConfiguration.Configuration
                .UseMemoryStorage()
                .UseActivator(new CheatingJobActivator(container))
                .UseColouredConsoleLogProvider();
            foreach (var job in container.Jobs)
            {
                RecurringJob.AddOrUpdate(() => job.Execute(), Cron.Minutely);
            }

            using var backgroundServer = new BackgroundJobServer();
            Console.WriteLine("Background processing started");
            Console.ReadLine();
        }
    }
}