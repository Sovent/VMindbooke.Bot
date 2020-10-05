using System;
using System.Collections.Generic;
using RestSharp;
using VMindbooke.SDK.Model;

namespace VMindbooke.SDK
{
    public class VMindbooke : IVMindbooke
    {
        private JsonRestClient _jsonRestClient;

        private static IDictionary<string, string> GetAuthHeaders(Credentials credentials) =>
            new Dictionary<string, string> {["Authorization"] = credentials.Token};

        private static IDictionary<string, string> GetPaginationParams(int? skip, int? take) => 
            new Dictionary<string, string> {["skip"] = skip?.ToString(), ["take"] = take?.ToString()};

        public VMindbooke(string baseUrl) => 
            _jsonRestClient = new JsonRestClient(baseUrl);
        
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
        
        public (User, Credentials) CreateUser(NewUser newUser) =>
            _jsonRestClient.MakeRequest<CreatedUser, NewUser>(
                Method.POST, "/users", newUser).Decompose();
        
        public int CreatePost(Credentials credentials, int userId, NewPost newPost) =>
            _jsonRestClient.MakeRequest<int, NewPost>(
                Method.POST, "/users", newPost,
                null, GetAuthHeaders(credentials));
        
        public void LikePost(Credentials credentials, int postId) =>
            _jsonRestClient.MakeRequest(
                Method.POST, $"/posts/{postId}/likes",
                null, GetAuthHeaders(credentials));
        
        public void CreateComment(Credentials credentials, int postId, NewComment newComment) =>
            _jsonRestClient.MakeRequest<NewComment>(
                Method.POST, $"/posts/{postId}/comments", newComment,
                null, GetAuthHeaders(credentials));
        
        public void LikeComment(Credentials credentials, int postId, Guid commentId) =>
            _jsonRestClient.MakeRequest(
                Method.POST, $"/users/{postId}/comments/{commentId}/likes",
                null, GetAuthHeaders(credentials));
        
        public void CreateReply(Credentials credentials, int postId, Guid commentId, NewComment newComment) =>
            _jsonRestClient.MakeRequest<NewComment>(
                Method.POST, $"/users/{postId}/comments/{commentId}/replies", newComment,
                null, GetAuthHeaders(credentials));
    }
}
