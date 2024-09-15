using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using RestSharp;
using Newtonsoft.Json;
using Polly;
using Serilog;

namespace VMindbooke.SDK
{
    public class VMindbookeClient : IVMindbookeClient
    {
        public VMindbookeClient(string vmindbookeBaseUrl)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("client.log")
                .WriteTo.Console()
                .CreateLogger();
            
            _restClient = new RestClient(vmindbookeBaseUrl);
        }
        public IReadOnlyCollection<User> GetUsers()
        {
            var resource = "users";
            return Get<IReadOnlyCollection<User>>(resource);
        }
        public User GetUser(int userId)
        {
            var resource = $"users/{userId}";
            return Get<User>(resource);
        }
        public Post GetPost(int postId)
        {
            var resource = $"posts/{postId}";
            return Get<Post>(resource);
        }
        public IReadOnlyCollection<Post> GetPosts()
        {
            var resource = $"posts";
            return Get<IReadOnlyCollection<Post>>(resource);
        }

        public IReadOnlyCollection<Post> GetUserPosts(int userId)
        {
            var resource = $"users/{userId}/posts";
            return Get<IReadOnlyCollection<Post>>(resource);
        }
        
        public void Comment(CommentContent commentContent, int postId, string token)
        {
            var resource = $"posts/{postId}/comments";
            Post<string>(resource, commentContent, token);
        }
        public void Reply(ReplyContent replyContent, int postId, Guid commentId, string token)
        {
            var resource = $"posts/{postId}/comments/{commentId}/replies";
            Post<int>(resource, replyContent, token);
        }
        public int Post(PostContent postContent, int userId, string token)
        {
            var resource = $"users/{userId}/posts";
            return Post<int>(resource, postContent, token);
        }

        private T Get<T>(string resource)
        {
            var request = new RestRequest(resource, Method.GET);
            
            var response = ResponseWithRetry(request, resource);
            return Deserialization<T>(response, resource);
        }
        
        private T Post<T>(string resource, IContent bodyContent, string token)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(bodyContent);
            request.AddHeader("Authorization", token);

            var response = ResponseWithRetry(request, resource);
            if(response.IsSuccessful)
                return Deserialization<T>(response, resource);
            else
            {
                Log.Error($"Retry count ({_retryMaxCount}) to /{resource} exceeded");
                return default(T);
            }
        }
        
        public void Retry(DelegateResult<IRestResponse> result, int retryNumber, Polly.Context context,
            RestSharp.Method method, string resource)
        {
            Log.Information($"{retryNumber} retry of {method} to /{resource}. " +
                            $"Exception: {result.Result.StatusCode}");
        }
        
        public IRestResponse ResponseWithRetry(RestRequest request, string resource)
        {
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => httpStatusCodesToRetry.Contains(r.StatusCode))
                .Retry(_retryMaxCount,
                    (exception, i, context) 
                        => Retry(exception, i, context, request.Method, resource));
            return retryPolicy.Execute(() => _restClient.Execute<int>(request));
        }
        
        public T Deserialization<T>(IRestResponse response, string resource)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch
            {
                Log.Error($"Response deserialization error from /{resource}");
                return default(T);
            }
        }
        
        HttpStatusCode[] httpStatusCodesToRetry = {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout // 504
        }; 

        private readonly RestClient _restClient;
        private readonly int _retryMaxCount = 1;
    }
}