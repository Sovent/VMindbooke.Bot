using RestSharp;

namespace VMindBooke.SDK.Application
{
    public class VMindBookeFactory
    {
        public static VMindBookeClient VMindBookeClientBuild(string vMindBookeBaseUrl)
        {
            var restClient = new RestClient(vMindBookeBaseUrl);
            var vMindBookeClient =
                new VMindBookeClient(new UserService(restClient), new PostService(restClient));
            return vMindBookeClient;
        }
    }
}