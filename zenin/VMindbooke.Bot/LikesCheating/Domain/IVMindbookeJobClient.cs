namespace LikesCheating.Domain
{
    public interface IVMindbookeJobClient
    {
        void StartJobs(int userId, string token, Thresholds thresholds);
        void StopJobs();
    }
}