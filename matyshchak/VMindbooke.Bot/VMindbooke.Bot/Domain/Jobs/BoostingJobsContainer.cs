using System;
using System.Collections.Generic;
using Hangfire;

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
            _boostingJobs();
        }

        public void StopJobs()
        {
            foreach (var jobId in _jobIds)
            {
                RecurringJob.RemoveIfExists(jobId);
            }
        }
    }
}