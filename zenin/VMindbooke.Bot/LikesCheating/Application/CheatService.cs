using System;
using LikesCheating.Domain;

namespace LikesCheating.Application
{
    public class CheatService : ICheatService
    {
        public CheatService()
        {
            _jobClient = new VMindbookeJobClient();
        }
        public void StartCheating(int userId, string token)
        {
            _jobClient.StartCheatingJobs(userId, token);
        }

        public void StopCheating()
        {
            _jobClient.StopCheatingJobs();
        }

        private  readonly VMindbookeJobClient _jobClient;
    }
}