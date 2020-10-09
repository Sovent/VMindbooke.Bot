using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using VMindbooke.SDK;
using VMindbooke.SDK.Model;

namespace VMindbooke.RainbowKarma
{
    public class RainbowClient
    {
        private readonly IVMindbookeClient _vMindbooke;
        private readonly NewUser _newUser;
        private UserCredentials _credentials;

        public RainbowClient()
        {
            IConfiguration cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _vMindbooke = new VMindbookeClient(cfg["ApiBaseUrl"]);
            _credentials = new UserCredentials(int.Parse(cfg["UserId"]), cfg["UserToken"]);
            _newUser = new NewUser(cfg["NewUserName"]);
        }

        public IEnumerable<Post> GetPosts() =>
            _vMindbooke.GetPosts();

        public IEnumerable<User> GetUsers() =>
            _vMindbooke.GetUsers();

        public IEnumerable<Post> GetUserPosts(User user) =>
            _vMindbooke.GetUserPosts(user.Id);

        public void AutoRegister() =>
            _credentials = _vMindbooke.CreateUser(_newUser);

        public void Comment(Post post) =>
            _vMindbooke.CreateComment(_credentials, post.Id,
                new NewComment(Keki.GetCleverComment(post.Content)));

        public void Reply(Post post, Comment comment) =>
            _vMindbooke.CreateReply(_credentials, post.Id, comment.Id,
                new NewComment(Keki.GetCleverReply(comment.Content)));

        public void Repost(Post post) =>
            _vMindbooke.CreatePost(_credentials, _credentials.Id,
                new NewPost(Keki.GetCleverTitle(post.Title), post.Content));
    }
}
