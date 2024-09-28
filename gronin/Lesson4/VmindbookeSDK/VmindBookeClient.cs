using System;
using System.Collections.Generic;

using RestSharp;

using VMindbookeSDK.Entities;

namespace VmindbookeSDK
{
    public class VmindBookeClient:IVmindbookeClient
    {
        private RestClientRequester _restClientRequester;

        private static IDictionary<string, string> GetAuthHeaders(UserCredentials credentials) =>
            new Dictionary<string, string> {["Authorization"] = credentials.Token};

        private static IDictionary<string, string> MakePaginationParams(int? skip, int? take) => 
            new Dictionary<string, string> {["skip"] = skip?.ToString(), ["take"] = take?.ToString()};

        public VmindBookeClient(string baseUrl) => 
            _restClientRequester = new RestClientRequester(baseUrl);
        
        public IEnumerable<User> GetUsers(int? skip = null, int? take = null) => 
            _restClientRequester.SendRequestParseResult<IEnumerable<User>>(
                Method.GET,
                "/users",
                MakePaginationParams(skip, take));
        
        public User GetUser(int userId) =>
            _restClientRequester.SendRequestParseResult<User>(
                Method.GET,
                $"/users/{userId}");
        
        public IEnumerable<Post> GetUserPosts(int userId) =>
            _restClientRequester.SendRequestParseResult<IEnumerable<Post>>(
                Method.GET,
                $"/users/{userId}/posts");
        
        public IEnumerable<Post> GetPosts(int? skip = null, int? take = null) =>
            _restClientRequester.SendRequestParseResult<IEnumerable<Post>>(
                Method.GET,
                "/posts",
                MakePaginationParams(skip, take));
        
        public Post GetPost(int postId) =>
            _restClientRequester.SendRequestParseResult<Post>(
                Method.GET,
                $"/posts/{postId}");

        public UserCredentials RegisterUser(NewUser newUser) =>
            _restClientRequester.SendRequestParseResult<UserCredentials, NewUser>(
                Method.POST,
                "/users",
                newUser);
        
        public int CreatePost(UserCredentials credentials, int userId, NewPost newPost) =>
             int.Parse(_restClientRequester.SendRequest<NewPost>(
                 Method.POST,
                 $"/users/{userId}/posts",
                 newPost,
                 null,
                 GetAuthHeaders(credentials)).Content);
        
        public void LikePost(UserCredentials credentials, int postId) =>
            _restClientRequester.SendRequest(
                Method.POST,
                $"/posts/{postId}/likes",
                null,
                GetAuthHeaders(credentials));
        
        public void CreateComment(UserCredentials credentials, int postId, NewComment newComment) =>
            _restClientRequester.SendRequest(
                Method.POST,
                $"/posts/{postId}/comments",
                newComment,
                null,
                GetAuthHeaders(credentials));
        
        public void LikeComment(UserCredentials credentials, int postId, Guid commentId) =>
            _restClientRequester.SendRequest(
                Method.POST,
                $"/posts/{postId}/comments/{commentId}/likes",
                null,
                GetAuthHeaders(credentials));
        
        public void CreateReply(UserCredentials credentials, int postId, Guid commentId, NewComment reply) =>
            _restClientRequester.SendRequest(
                Method.POST,
                $"/posts/{postId}/comments/{commentId}/replies",
                reply,
                null, 
                GetAuthHeaders(credentials));
    }

    
}