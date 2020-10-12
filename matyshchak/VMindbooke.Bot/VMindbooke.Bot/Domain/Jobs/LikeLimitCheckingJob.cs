using System;
using Hangfire;
using Serilog;
using Usage.Domain.ValueObjects;
using Usage.Domain.ValueObjects.LikeThresholds;

namespace Usage.Domain.Jobs
{
    public class LikeLimitCheckingJob : IBoostingJob
    {
        private readonly BoostingJobsContainer _boostingJobsContainer;
        private readonly IVmClient _vmClient;
        private readonly UserCredentials _userCredentials;
        private readonly UserLikesThreshold _userLikesThreshold;

        public LikeLimitCheckingJob(IVmClient vmClient,
            UserCredentials userCredentials,
            UserLikesThreshold userLikesThreshold,
            BoostingJobsContainer boostingJobsContainer)
        {
            _vmClient = vmClient;
            _userCredentials = userCredentials;
            _userLikesThreshold = userLikesThreshold;
            _boostingJobsContainer = boostingJobsContainer;
        }

        private bool IsLimitExceeded()
        {
            var numberOfUserLikes = _vmClient.GetUser(_userCredentials.Id).Likes.Count;
            return numberOfUserLikes > _userLikesThreshold.Value;
        }

        public void Execute()
        {
            if (IsLimitExceeded())
            {
                _boostingJobsContainer.StopJobs();
                var delay = DateTime.Today.AddDays(1) - DateTime.Now;
                Log.Information($"Scheduling next boosting jobs start with delay: {delay}");
                BackgroundJob.Schedule(() => _boostingJobsContainer.StartJobs(), delay);
            }
        }
    }
}