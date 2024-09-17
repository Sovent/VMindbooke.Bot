using System.Collections.Generic;
using VMindBooke.SDK.Application;
using VMindBooke.SDK.Domain;

namespace CarmaFucker.Tests
{
    public class PostServiceMock : IPostService
    {
        public Post GetPost(int id)
        {
            return FakeDataGenerator.GetPost(id);
        }

        public IEnumerable<Post> GetAllPosts()
        {
            var postList = new List<Post> {GetPost(1), GetPost(2), GetPost(3)};
            return postList;
        }

        public void LikePost(Post post, User actor)
        {
        }

        public void CommentPost(Post post, User actor, string content)
        {
        }

        public void ReplyComment(Post post, Comment comment, User actor, string content)
        {
        }

        public void LikeComment(Post post, Comment comment, User actor)
        {
        }
    }
}