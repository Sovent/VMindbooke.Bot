using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Polly;
using Polly.Retry;
using RestSharp;
using Serilog;

namespace VMBook.SDK
{
    public class VmbClient : IVmbClientRetryer, IVmbClientFilter
    {
        private readonly RestClient _restClient;
        private readonly string _authorizationToken;
        private readonly int _userId;
        public VmbClient(string baseUrl, int userId, string authorizationToken)
        {
            _restClient = new RestClient(baseUrl);
            _userId = userId;
            _authorizationToken = authorizationToken;
        }
        public IReadOnlyCollection<User> GetUsers()
        {
            var request = new RestRequest("users");
            Log.Information($"Requesting all users");
            var response = _restClient.Get<IEnumerable<User>>(request);
            return response.Data.ToArray();
        }

        public User GetUser(int id)
        {
            var request = new RestRequest($"users/{id}");
            Log.Information($"Requesting user with id: {id}");
            var response = _restClient.Get<User>(request);
            return response.Data;
        }
   
        public IReadOnlyCollection<Post> GetPosts()
        {
            var request = new RestRequest("posts");
            var response = _restClient.Get<IEnumerable<Post>>(request);
            return response.Data.ToArray();
        }

        public IRestResponse WriteComment(int postId)
        {
            var request = MakeAuthorizedRequest($"posts/{postId}/comments", 
                new {content = "I am watching you!"});
            return _restClient.Post(request);
        }
        
        private RestRequest MakeAuthorizedRequest(string requestUrl, object bodyContent)
        {
            var request = new RestRequest(requestUrl);
            request.AddHeader("Authorization", _authorizationToken);
            request.AddJsonBody(bodyContent);
            return request;
        }

        public IRestResponse WriteReplyUnderComment(int postId, Guid commentId)
        {
            var request = MakeAuthorizedRequest($"posts/{postId}/comments/{commentId}/replies",
                new {content = "Catch up comment from troll factory!"});

            return _restClient.Post(request);
        }

        public IRestResponse MakeCopyOfPost(string postTitle, string postContent)
        {
            var request = MakeAuthorizedRequest($"users/{_userId}/posts",
                new {title = postTitle, content = postContent});
            
            return _restClient.Post(request);
        }

        public IReadOnlyCollection<Post> GetUserPosts(int userId)
        {
            var request = new RestRequest($"users/{userId}/posts");
            var response = _restClient.Get<IEnumerable<Post>>(request);
            return response.Data.ToArray();
        }
    }
}