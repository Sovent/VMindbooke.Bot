using Microsoft.Extensions.Configuration;

namespace LikesCheating.Infrastructure
{
    public class Configuration : IConfiguration
    {
        public Configuration()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
        public string GetValue(string key)
        {
            return _configuration[key];
        }

        private readonly IConfigurationRoot _configuration;
    }
}