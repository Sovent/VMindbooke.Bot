using System;
using System.Collections;
using System.Collections.Generic;
using RestSharp;
using VMindbooke.SDK.Model;

namespace VMindbooke.SDK
{
    public class VMindbooke : IVMindbooke
    {
        private JsonRestClient _jsonRestClient;

        private static ICollection<KeyValuePair<string, string>> GetAuthHeaders(Credentials credentials) =>
            new[] { new KeyValuePair<string, string>("Authorization", credentials.Token) };

        public VMindbooke(string baseUrl) => 
            _jsonRestClient = new JsonRestClient(baseUrl);
        
        public IEnumerable<User> GetUsers(int? skip = null, int? take = null) => 
            throw new NotImplementedException(); // TODO: GET /users?skip={skip}&take={take}
        
        public User GetUser(int userId) =>
            _jsonRestClient.MakeJsonRequest<User>(Method.GET, $"/users/{userId}");
        
        public IEnumerable<Post> GetUserPosts(int userId) =>
            _jsonRestClient.MakeJsonRequest<IEnumerable<Post>>(Method.GET, $"/users/{userId}");
        
        public IEnumerable<Post> GetPosts(int? skip = null, int? take = null) =>
            throw new NotImplementedException(); // TODO: GET /posts?skip={skip}&take={take}
        
        public Post GetPost(int postId) =>
            _jsonRestClient.MakeJsonRequest<Post>(Method.GET, $"/posts/{postId}");
        
        public (User, Credentials) CreateUser(NewUser newUser) =>
            _jsonRestClient.MakeJsonRequest<CreatedUser, NewUser>(
                Method.POST, "/users", newUser).Decompose();
        
        public int CreatePost(Credentials credentials, int userId, NewPost newPost) =>
            _jsonRestClient.MakeJsonRequest<int, NewPost>(
                Method.POST, "/users",
                newPost, GetAuthHeaders(credentials));
        
        public void LikePost(Credentials credentials, int postId) =>
            _jsonRestClient.MakeJsonRequest(
                Method.POST, $"/posts/{postId}/likes",
                GetAuthHeaders(credentials));
        
        public void CreateComment(Credentials credentials, int postId, NewComment newComment) =>
            _jsonRestClient.MakeJsonRequest<NewComment>(
                Method.POST, $"/posts/{postId}/comments",
                newComment, GetAuthHeaders(credentials));
        
        public void LikeComment(Credentials credentials, int postId, Guid commentId) =>
            _jsonRestClient.MakeJsonRequest(
                Method.POST, $"/users/{postId}/comments/{commentId}/likes",
                GetAuthHeaders(credentials));
        
        public void CreateReply(Credentials credentials, int postId, Guid commentId, NewComment newComment) =>
            _jsonRestClient.MakeJsonRequest<NewComment>(
                Method.POST, $"/users/{postId}/comments/{commentId}/replies",
                newComment, GetAuthHeaders(credentials));
    }
}
