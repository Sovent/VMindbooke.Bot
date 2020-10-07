using System.Collections.Generic;

namespace VMindbooke.SDK
{
    public interface IVMindbookeClient
    {
        User GetUser(int userId);
        IReadOnlyCollection<User> GetUsers();
        Post GetPost(int postId);
        IReadOnlyCollection<Post> GetPosts();
        
        int Comment(CommentContent commentContent, int postId, string token);
        int Reply(ReplyContent replyContent, int postId, int commentId, string token);
        int Post(PostContent postContent, int userId, string token);
    }
}