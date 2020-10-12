using System;
using Hangfire;

namespace VMindbooster
{
    public class CheatingJobActivator : JobActivator
    {
        private readonly CheatingJobContainer _cheatingJobContainer;

        public CheatingJobActivator(CheatingJobContainer cheatingJobContainer)
        {
            _cheatingJobContainer = cheatingJobContainer;
        }
        
        public override object ActivateJob(Type jobType)
        {
            return _cheatingJobContainer.GetByType(jobType);
        }
    }
}