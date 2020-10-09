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

            BackgroundJob.Enqueue(() => StartDailyJobs(userId, token));
            RecurringJob.AddOrUpdate(
                _generalJobId,
                () => StartDailyJobs(userId, token),
                Cron.Daily);

            using (var backgroundServer = new BackgroundJobServer())
            {
                Log.Information("Background server started");
                while (true);
            }
        }

        public void StopCheatingJobs()
        {
            Log.Information("Background server stopping...");
            RecurringJob.RemoveIfExists(_generalJobId);
        }
        
        public void StartDailyJobs(int userId, string token)
        {
            Log.Information("Daily jobs started");
            RecurringJob.RemoveIfExists(_dailyJobsId);
            RecurringJob.AddOrUpdate(
                _dailyJobsId,
                () => CheckJobsRunningCondition(userId, token),
                Cron.Minutely);
        }
        
        public void CheckJobsRunningCondition(int userId, string token)
        {
            if (!_cheatEngine.CheckUserReachedLikesThreshold(userId))
            {
                DoCheatingJobs(userId, token);
            }
            else
            {
                RecurringJob.RemoveIfExists(_dailyJobsId);
                Log.Information($"Target likes count on user {userId} reached. Stopping daily jobs...");
            }
        }

        public void DoCheatingJobs(int userId, string token)
        {
            try
            { _cheatEngine.CommentIfLikesMoreThanThreshold(token); }
            catch (Exception e)
            { Log.Error($"Method 'CommentIfLikesMoreThanThreshold' down: {e}"); }
            try
            { _cheatEngine.ReplyIfLikesMoreThanThreshold(token); }
            catch (Exception e)
            { Log.Error($"Method 'ReplyIfLikesMoreThanThreshold' down: {e}"); }
            try
            { _cheatEngine.DuplicatePostIfLikesMoreThanThreshold(userId, token); }
            catch (Exception e)
            { Log.Error($"Method 'DuplicatePostIfLikesMoreThanThreshold' down: {e}"); }
            try
            { _cheatEngine.DuplicateMostSuccessfulPostIfLikesMoreThanThreshold(userId, token); }
            catch (Exception e)
            { Log.Error($"Method 'DuplicateMostSuccessfulPostIfLikesMoreThanThreshold' down: {e}"); }
        }

        private readonly string _dailyJobsId = "daily-jobs-id";
        private readonly string _generalJobId = "general-job-id";
        private readonly Cheating _cheatEngine;
    }
}