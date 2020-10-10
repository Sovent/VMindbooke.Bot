using VMindbookeBooster.Entities;

namespace VMindbookeBooster
{
    public interface ILikeLimitChecker
    {
        public bool IsLimitExceeded(int userLikesThreshold);
    }

    public class LikeLimitChecker : ILikeLimitChecker
    {
        public LikeLimitChecker(IVmClient vmClient, UserCredentials userCredentials)
        {
            _vmClient = vmClient;
            _userCredentials = userCredentials;
        }
        
        private readonly IVmClient _vmClient;
        private readonly UserCredentials _userCredentials;

        public bool IsLimitExceeded(int userLikesThreshold)
        {
            var numberOfUserLikes = _vmClient.GetUser(_userCredentials.Id).Likes.Count;
            return numberOfUserLikes > userLikesThreshold;
        }
    }
}