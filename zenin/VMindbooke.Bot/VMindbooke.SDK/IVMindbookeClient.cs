using System;
using System.Collections.Generic;

namespace VMindbooke.SDK
{
    public interface IVMindbookeClient
    {
        User GetUser(int userId);
        IReadOnlyCollection<User> GetUsers();
        Post GetPost(int postId);
        IReadOnlyCollection<Post> GetPosts();
        IReadOnlyCollection<Post> GetUserPosts(int userId);
        
        void Comment(CommentContent commentContent, int postId, string token);
        void Reply(ReplyContent replyContent, int postId, Guid commentId, string token);
        int Post(PostContent postContent, int userId, string token);
    }
}