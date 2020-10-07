using LikesCheating.Domain;

namespace LikesCheating.Application
{
    public interface ICheatService
    {
        void StartCheating(int userId);
        void StopCheating();
    }
}