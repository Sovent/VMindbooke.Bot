namespace VMindBooke.SDK.Application
{
    public class VMindBookeClient
    {
        public IUserService UserService { get; }
        public IPostService PostService { get; }

        public VMindBookeClient(IUserService userService, IPostService postService)
        {
            UserService = userService;
            PostService = postService;
        }
    }
}