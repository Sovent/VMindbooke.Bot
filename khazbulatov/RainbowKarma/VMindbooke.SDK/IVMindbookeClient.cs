using System;
using System.Collections.Generic;
using VMindbooke.SDK.Model;

namespace VMindbooke.SDK
{
    public interface IVMindbookeClient
    {
        IEnumerable<User> GetUsers(int? skip = null, int? take = null);
        User GetUser(int userId);
        IEnumerable<Post> GetUserPosts(int userId);
        IEnumerable<Post> GetPosts(int? skip = null, int? take = null);
        Post GetPost(int postId);
        
        UserCredentials CreateUser(NewUser newUser);
        int CreatePost(UserCredentials credentials, int userId, NewPost newPost);
        void LikePost(UserCredentials credentials, int postId);
        void CreateComment(UserCredentials credentials, int postId, NewComment newComment);
        void LikeComment(UserCredentials credentials, int postId, Guid commentId);
        void CreateReply(UserCredentials credentials, int postId, Guid commentId, NewComment newComment);
    }
}
