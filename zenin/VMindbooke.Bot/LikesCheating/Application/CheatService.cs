using System;
using LikesCheating.Domain;
using Microsoft.Extensions.Configuration;
using VMindbooke.SDK;

namespace LikesCheating.Application
{
    public class CheatService : ICheatService
    {
        public CheatService()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var client = new VMindbookeClient(_configuration["VMindbookeUrl"]);
            _jobClient = new VMindbookeJobClient(client);
        }
        public void StartCheating(int userId)
        {
            var token = _configuration["VMindbooleUserToken"];
            var thresholds = new Thresholds(
                Convert.ToInt32(_configuration["PostThresholdToComment"]),
                Convert.ToInt32(_configuration["PostThresholdToDuplicate"]),
                Convert.ToInt32(_configuration["CommentThreshold"]),
                Convert.ToInt32(_configuration["UserThreshold"])
                );
            _jobClient.StartJobs(userId, token, thresholds);
        }

        public void StopCheating()
        {
            _jobClient.StopJobs();
        }

        private readonly IConfigurationRoot _configuration;
        private  readonly VMindbookeJobClient _jobClient;
    }
}