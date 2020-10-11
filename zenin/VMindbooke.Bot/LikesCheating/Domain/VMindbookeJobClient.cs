using System;
using Hangfire;
using Hangfire.MemoryStorage;
using LikesCheating.Infrastructure;

namespace LikesCheating.Domain
{
    public class VMindbookeJobClient : IVMindbookeJobClient
    {
        public VMindbookeJobClient()
        {
            _logger = new Logger("job-client.log");
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
                _logger.Information("Background server started");
                while (true);
            }
        }

        public void StopCheatingJobs()
        {
            _logger.Information("Background server stopping...");
            RecurringJob.RemoveIfExists(_generalJobId);
        }
        
        public void StartDailyJobs(int userId, string token)
        {
            _logger.Information("Daily jobs started");
            RecurringJob.RemoveIfExists(_dailyJobsId);
            RecurringJob.AddOrUpdate(
                _dailyJobsId,
                () => RunCheatingJobs(userId, token),
                Cron.Minutely);
        }
        
        public void RunCheatingJobs(int userId, string token)
        {
            if (!_cheatEngine.CheckUserReachedLikesThreshold(userId))
            {
                DoCheatingJobs(userId, token);
            }
            else
            {
                RecurringJob.RemoveIfExists(_dailyJobsId);
                _logger.Information($"Target likes count on user {userId} reached. Stopping daily jobs...");
            }
        }

        public void DoCheatingJobs(int userId, string token)
        {
            DoJob(_cheatEngine.CommentIfLikesMoreThanThreshold, token); 
            DoJob(_cheatEngine.ReplyIfLikesMoreThanThreshold, token);
            DoJob(_cheatEngine.DuplicatePostIfLikesMoreThanThreshold, userId, token);
            DoJob(_cheatEngine.DuplicateMostSuccessfulPostIfLikesMoreThanThreshold, userId, token);
        }

        public void DoJob(Action<int, string> action, int userId, string token)
        {
            try
            { action(userId, token); }
            catch (Exception e)
            { _logger.Error($"Method {action.Method.Name} down: {e}"); }
        }
        
        public void DoJob(Action<string> action, string token)
        {
            try
            { action(token); }
            catch (Exception e)
            { _logger.Error($"Method {action.Method.Name} down: {e}"); }
        }

        private readonly ILogger _logger;
        private readonly ICheating _cheatEngine;
        private readonly string _dailyJobsId = "daily-jobs-id";
        private readonly string _generalJobId = "general-job-id";
    }
}