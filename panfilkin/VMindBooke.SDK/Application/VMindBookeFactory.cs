using RestSharp;

namespace VMindBooke.SDK.Application
{
    public class VMindBookeFactory
    {
        public static IVMindBookeClient VMindBookeClientBuild(string vMindBookeBaseUrl)
        {
            var restClient = new RestClient(vMindBookeBaseUrl);
            var a = new VMindBookeClient(restClient, new UserService(restClient), new PostService(restClient));
            return a;
        }
    }
}