using System;
using Microsoft.Extensions.Configuration;

namespace Usage
{
    public class Configuration
    {
        public string ServerAddress { get; }
        public string Token { get; }
        public int WriteCommentThreshold { get; }
        public int ReplyCommentThreshold { get; }
        public int CopyCommentThreshold { get; }
        public int MostLikedCommentThreshold { get; }
        public int StopBoostThreshold { get; }
        public string LogName { get; }

        public string Local { get; }

        public int Retry { get; }
        public int UserIdMaxValue { get; }

        public Configuration(string config)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile(config).Build();
            ServerAddress = configuration["Server address"];
            Token = configuration["Bot Initial Token"];
            WriteCommentThreshold = Convert.ToInt32(configuration["Write Comment Threshold"]);
            ReplyCommentThreshold = Convert.ToInt32(configuration["Reply Comment Threshold"]);
            CopyCommentThreshold = Convert.ToInt32(configuration["Copy Post Threshold"]);
            MostLikedCommentThreshold = Convert.ToInt32(configuration["Most Daily Liked Post Threshold"]);
            StopBoostThreshold = Convert.ToInt32(configuration["Stop Boost Threshold"]);
            LogName = configuration["Log File Name"];
            Local = configuration["Local"];
            Retry = Convert.ToInt32(configuration["Retry"]);
            UserIdMaxValue = Convert.ToInt32(configuration["UserIdMaxValue"]);
        }
    }
}