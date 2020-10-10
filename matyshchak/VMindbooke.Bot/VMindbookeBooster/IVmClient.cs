using System;
using System.Collections.Generic;
using VMindbookeBooster.Entities;

namespace VMindbookeBooster
{
    public interface IVmClient
    {
        User Register(UserName userName);
        IReadOnlyCollection<User> GetAllUsers();
        IReadOnlyCollection<User> GetUsers(int take, int skip = 0);
        User GetUser(int userId);
        IReadOnlyCollection<Post> GetUserPosts(int userId);
        IEnumerable<Post> GetAllPosts();
        IEnumerable<Comment> GetAllComments();
        IReadOnlyCollection<Post> GetPosts(int take, int skip = 0);
        int Post(int userId, string userToken, PostContent postContent);
        void CommentPost(int userId, string userToken, int postId, CommentContent comment);
        void ReplyToComment(string userToken, int postId, Guid commentId, CommentContent reply);
    }
}