using System;
using System.Collections.Generic;
using VMindbooke.SDK.Model;

namespace VMindbooke.SDK
{
    public interface IVMindbooke
    {
        IEnumerable<User> GetUsers(int? skip = null, int? take = null);
        User GetUser(int userId);
        IEnumerable<Post> GetUserPosts(int userId);
        IEnumerable<Post> GetPosts(int? skip = null, int? take = null);
        Post GetPost(int postId);
        
        (User, Credentials) CreateUser(NewUser newUser);
        int CreatePost(Credentials credentials, int userId, NewPost newPost);
        void LikePost(Credentials credentials, int postId);
        void CreateComment(Credentials credentials, int postId, NewComment newComment);
        void LikeComment(Credentials credentials, int postId, Guid commentId);
        void CreateReply(Credentials credentials, int postId, Guid commentId, NewComment newComment);
    }
}
