using RestSharp;
using VMindBooke.SDK.Domain;

namespace VMindBooke.SDK.Application
{
    public class VMindBookeClient : IVMindBookeClient
    {
        private RestClient _restClient;
        public IUserService UserService { get; }
        public IPostService PostService { get; }
        
        public VMindBookeClient(RestClient restClient, UserService userService, PostService postService)
        {
            _restClient = restClient;
            UserService = userService;
            PostService = postService;
        }
        
        
    }
}