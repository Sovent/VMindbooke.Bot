using System;
using LikesCheating.Domain;

namespace LikesCheating.Application
{
    public class CheatService : ICheatService
    {
        public CheatService(VMindbookeJobClient jobClient)
        {
            _jobClient = jobClient;
        }
        public void StartCheating(CheatingUserContent userContent)
        {
            _jobClient.StartCheatingJobs(userContent.UserId, userContent.Token);
        }

        public void StopCheating()
        {
            _jobClient.StopCheatingJobs();
        }

        private  readonly VMindbookeJobClient _jobClient;
    }
}