using System;
using Serilog;
using Hangfire;
using Hangfire.MemoryStorage;

namespace LikesCheating.Domain
{
    public class VMindbookeJobClient : IVMindbookeJobClient
    {
        public VMindbookeJobClient()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("job_client.log")
                .WriteTo.Console()
                .CreateLogger();
            
            _cheatEngine = new Cheating();
        }

        public void StartCheatingJobs(int userId, string token)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();
            RecurringJob.AddOrUpdate(
                () => DoCheatingJobs(userId, token),
                Cron.Minutely);

            using (var backgroundServer = new BackgroundJobServer())
            {
                Log.Information("Background server started.");
                while (!_endCheating);
            }
        }

        public void StopCheatingJobs()
        {
            Log.Information("Background server stopping...");
            _endCheating = true;
        }
        
        public void DoCheatingJobs(int userId, string token)
        {
            if (!_cheatEngine.CheckUserReachedLikesThreshold(userId))
            {
                try
                { _cheatEngine.CommentIfLikesMoreThanThreshold(token); }
                catch (Exception e)
                { Log.Error($"Method 'CommentIfLikesMoreThanThreshold' down: {e}"); }
            }
            else
                Log.Information($"Target likes count on user {userId} reached");
        }

        private static bool _endCheating;
        private readonly Cheating _cheatEngine;
    }
}