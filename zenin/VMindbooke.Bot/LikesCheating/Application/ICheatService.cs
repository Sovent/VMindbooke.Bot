using LikesCheating.Domain;

namespace LikesCheating.Application
{
    public interface ICheatService
    {
        void StartCheating(CheatingUserContent userContent);
        void StopCheating();
    }
}