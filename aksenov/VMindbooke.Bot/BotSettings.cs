using System;
using System.IO;
using Newtonsoft.Json;

namespace VMindbooke.Bot
{
    public class BotSettings
    {
        public BotSettings(string serverAddress, string userToken, int userId, string userName, int likeLimitForPostToMakeComment, int likeLimitForCommentToMakeReply, int likeLimitForPostToCopy, int likeLimitForUserToCopyPost, int likeLimitToCompleteProcess)
        {
            ServerAddress = serverAddress;
            UserToken = userToken;
            UserId = userId;
            UserName = userName;
            LikeLimitForPostToMakeComment = likeLimitForPostToMakeComment;
            LikeLimitForCommentToMakeReply = likeLimitForCommentToMakeReply;
            LikeLimitForPostToCopy = likeLimitForPostToCopy;
            LikeLimitForUserToCopyPost = likeLimitForUserToCopyPost;
            LikeLimitToCompleteProcess = likeLimitToCompleteProcess;
        }

        public static BotSettings FromJsonFile(string filePath)
        {
            var settingInJson = File.ReadAllText(filePath);

            BotSettings settings = JsonConvert.DeserializeObject<BotSettings>(settingInJson);

            return settings;
        }

        public string ServerAddress { get; }    
        
        public string UserToken { get; }
        
        public int UserId { get; }
        
        public string UserName { get; }

        public int LikeLimitForPostToMakeComment { get; }

        public int LikeLimitForCommentToMakeReply { get; }

        public int LikeLimitForPostToCopy { get; }

        public int LikeLimitForUserToCopyPost { get; }

        public int LikeLimitToCompleteProcess { get; }
    }
}