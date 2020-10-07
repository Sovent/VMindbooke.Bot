using System.Collections.Generic;
using VMindBooke.SDK.Domain;

namespace VMindBooke.SDK.Application
{
    public interface IPostService
    {
        Post GetPost(int id);
        IReadOnlyCollection<Post> GetAllPosts();
        
        void LikePost(Post post, User actor);
        void CommentPost(Post post, User actor, string content);
        void ReplyComment(Post post, Comment comment, User actor, string content);
        void LikeComment(Post post, Comment comment, User actor);
    }
}