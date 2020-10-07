namespace VMindBooke.SDK.Application
{
    public interface IVMindBookeClient
    {
        IUserService UserService { get; }
        IPostService PostService { get; }
    }
}