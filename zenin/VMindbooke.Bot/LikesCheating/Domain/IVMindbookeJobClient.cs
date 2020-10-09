namespace LikesCheating.Domain
{
    public interface IVMindbookeJobClient
    {
        void StartCheatingJobs(int userId, string token);
        void StopCheatingJobs();
    }
}