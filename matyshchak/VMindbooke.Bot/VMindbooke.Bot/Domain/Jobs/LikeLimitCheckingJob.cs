using Usage.Domain.Entities;
using Usage.Domain.ValueObjects;

namespace Usage.Domain.Jobs
{
    public class LikeLimitCheckingJob : IBoostingJob

    {
        private readonly BoostingJobsContainer _boostingJobsContainer;

        public LikeLimitCheckingJob(IVmClient vmClient, UserCredentials userCredentials,
            BoostingJobsContainer boostingJobsContainer)
        {
            _vmClient = vmClient;
            _userCredentials = userCredentials;
            _boostingJobsContainer = boostingJobsContainer;
        }

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
                _boostingJobsContainer.StopJobs();
        }
    }
}