using System;
using System.Collections.Generic;
using Polly;
using RestSharp;
using VMindbooke.SDK.Model;

namespace VMindbooke.SDK
{
    public class VMindbookeClient : IVMindbookeClient
    {
        
        private readonly JsonRestClient _jsonRestClient;

        private static IDictionary<string, string> GetAuthHeaders(UserCredentials credentials) =>
            new Dictionary<string, string> {["Authorization"] = credentials.Token};

        private static IDictionary<string, string> GetPaginationParams(int? skip, int? take) => 
            new Dictionary<string, string> {["skip"] = skip?.ToString(), ["take"] = take?.ToString()};

        public VMindbookeClient(string baseUrl)
        {
            _jsonRestClient = new JsonRestClient(baseUrl);
        }

        public IEnumerable<User> GetUsers(int? skip = null, int? take = null) => 
            _jsonRestClient.MakeRequest<IEnumerable<User>>(Method.GET, $"/users",
                GetPaginationParams(skip, take));
        
        public User GetUser(int userId) =>
            _jsonRestClient.MakeRequest<User>(Method.GET, $"/users/{userId}");
        
        public IEnumerable<Post> GetUserPosts(int userId) =>
            _jsonRestClient.MakeRequest<IEnumerable<Post>>(Method.GET, $"/users/{userId}");
        
        public IEnumerable<Post> GetPosts(int? skip = null, int? take = null) =>
            _jsonRestClient.MakeRequest<IEnumerable<Post>>(Method.GET, $"/posts",
                GetPaginationParams(skip, take));
        
        public Post GetPost(int postId) =>
            _jsonRestClient.MakeRequest<Post>(Method.GET, $"/posts/{postId}");

        public UserCredentials CreateUser(NewUser newUser) =>
            _jsonRestClient.MakeRequest<UserCredentials, NewUser>(
                Method.POST, "/users", newUser);
        
        public int CreatePost(UserCredentials credentials, int userId, NewPost newPost) =>
            _jsonRestClient.MakeRequest<int, NewPost>(
                Method.POST, "/users", newPost,
                null, GetAuthHeaders(credentials));
        
        public void LikePost(UserCredentials credentials, int postId) =>
            _jsonRestClient.MakeRequest(
                Method.POST, $"/posts/{postId}/likes",
                null, GetAuthHeaders(credentials));
        
        public void CreateComment(UserCredentials credentials, int postId, NewComment newComment) =>
            _jsonRestClient.MakeRequest<NewComment>(
                Method.POST, $"/posts/{postId}/comments", newComment,
                null, GetAuthHeaders(credentials));
        
        public void LikeComment(UserCredentials credentials, int postId, Guid commentId) =>
            _jsonRestClient.MakeRequest(
                Method.POST, $"/users/{postId}/comments/{commentId}/likes",
                null, GetAuthHeaders(credentials));
        
        public void CreateReply(UserCredentials credentials, int postId, Guid commentId, NewComment newComment) =>
            _jsonRestClient.MakeRequest<NewComment>(
                Method.POST, $"/users/{postId}/comments/{commentId}/replies", newComment,
                null, GetAuthHeaders(credentials));
    }
}
