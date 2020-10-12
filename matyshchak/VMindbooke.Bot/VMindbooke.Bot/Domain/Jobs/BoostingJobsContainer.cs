using System;
using System.Collections.Generic;
using Hangfire;
using Serilog;

namespace Usage.Domain.Jobs
{
    public class BoostingJobsContainer
    {
        private Action _boostingJobs = () => { };
        private readonly HashSet<string> _jobIds = new HashSet<string>();
        
        public void Add<T>() where T : IBoostingJob
        {
            var jobId = Guid.NewGuid().ToString();
            _boostingJobs += () => RecurringJob.AddOrUpdate<T>(jobId, job => job.Execute(), Cron.Minutely);
            _jobIds.Add(jobId);
        }

        public void StartJobs()
        {
            Log.Information("Starting boosting jobs");
            _boostingJobs();
        }

        public void StopJobs()
        {
            Log.Information("Removing boosting jobs");
            foreach (var jobId in _jobIds)
            {
                BackgroundJob.Delete(jobId);
            }
        }
    }
}