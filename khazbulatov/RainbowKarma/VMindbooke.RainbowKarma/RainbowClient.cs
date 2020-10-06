using System;
using Microsoft.Extensions.Configuration;
using VMindbooke.SDK;
using VMindbooke.SDK.Model;

namespace VMindbooke.RainbowKarma
{
    public class RainbowClient
    {
        private IVMindbookeClient _vMindbooke;
        private UserCredentials _credentials;

        public RainbowClient()
        {
            IConfiguration cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();
            _vMindbooke = new VMindbookeClient(cfg["ApiBaseUrl"]);
            _credentials = new UserCredentials(int.Parse(cfg["UserId"]), cfg["UserToken"]);
        }

        public void Comment(Post post) =>
            _vMindbooke.CreateComment(_credentials, post.Id, new NewComment(Keki.GetCleverComment()));

        public void Reply(Post post, Comment comment) =>
            _vMindbooke.CreateReply(_credentials, post.Id, comment.Id, new NewComment(Keki.GetCleverReply()));

        public void Repost(Post post) =>
            _vMindbooke.CreatePost(_credentials, _credentials.Id, new NewPost(Keki.GetCleverTitle(), post.Content));
    }
}
