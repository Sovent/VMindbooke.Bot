using Hangfire;

namespace Usage.Domain.Jobs
{
    public interface IBoostingJob
    {
        public void Execute();
    }
}