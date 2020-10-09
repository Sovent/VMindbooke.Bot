using System;
using System.Collections.Generic;
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
            retryMaxCount = 3;
        }
        public IReadOnlyCollection<User> GetUsers()
        {
            var resource = "users";
            return GET<IReadOnlyCollection<User>>(resource);
        }
        public User GetUser(int userId)
        {
            var resource = $"users/{userId}";
            return GET<User>(resource);
        }
        public Post GetPost(int postId)
        {
            var resource = $"posts/{postId}";
            return GET<Post>(resource);
        }
        public IReadOnlyCollection<Post> GetPosts()
        {
            var resource = $"posts";
            return GET<IReadOnlyCollection<Post>>(resource);
        }

        public IReadOnlyCollection<Post> GetUserPosts(int userId)
        {
            var resource = $"users/{userId}/posts";
            return GET<IReadOnlyCollection<Post>>(resource);
        }
        
        public void Comment(CommentContent commentContent, int postId, string token)
        {
            var resource = $"posts/{postId}/comments";
            POST<string>(resource, commentContent, token);
        }
        public void Reply(ReplyContent replyContent, int postId, Guid commentId, string token)
        {
            var resource = $"posts/{postId}/comments/{commentId}/replies";
            POST<int>(resource, replyContent, token);
        }
        public int Post(PostContent postContent, int userId, string token)
        {
            var resource = $"users/{userId}/posts";
            return POST<int>(resource, postContent, token);
        }

        private T GET<T>(string resource)
        {
            var request = new RestRequest(resource, Method.GET);
            
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => !r.IsSuccessful)
                .Retry(retryMaxCount, 
                    (result, i) => Log.Information($"{i} retry of GET to /{resource}"));
            var response = retryPolicy.Execute(() => _restClient.Execute(request));

            var content = default(T);
            try
            { content = JsonConvert.DeserializeObject<T>(response.Content); }
            catch
            { Log.Error($"Retry count ({retryMaxCount}) of GET to /{resource} exceeded");}
            return content;
        }
        private T POST<T>(string resource, IContent bodyContent, string token)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(bodyContent);
            request.AddHeader("Authorization", token);
            
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => !r.IsSuccessful)
                .Retry(retryMaxCount, 
                    (result, i) => Log.Information($"{i} retry of POST to /{resource}"));
            var response = retryPolicy.Execute(() => _restClient.Execute<int>(request));
            var content = default(T);
            try
            { content = JsonConvert.DeserializeObject<T>(response.Content); }
            catch
            { Log.Error($"Retry count ({retryMaxCount}) of POST to /{resource} exceeded"); }
            return content;
        }
        
        private readonly RestClient _restClient;
        private readonly int retryMaxCount;
    }
}