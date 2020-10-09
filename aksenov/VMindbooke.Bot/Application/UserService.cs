using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Application
{
    public class UserService
    {
        private APIRequestsService _apiService;

        public UserService(APIRequestsService apiService)
        {
            _apiService = apiService;
        }

        public bool DoesUserExists(int userId)
        {
            var user = _apiService.GetUser(userId);
            if (user != null && user.IsValid())
                return true;

            return false;
        }

        public User RegisterUser(string userName)
        {
            return _apiService.CreateUser(userName);
        }
    }
}