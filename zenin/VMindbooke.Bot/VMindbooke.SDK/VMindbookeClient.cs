using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using Polly;
using Serilog;
using Serilog.Core;
using Serilog.Events;

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
            var request = new RestRequest(resource, Method.GET);
            
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => !r.IsSuccessful)
                .Retry(retryMaxCount, 
                    (result, i) => Log.Information($"{i} retry of GET to /{resource}"));
            var response = retryPolicy.Execute(() => _restClient.Execute(request));

            User[] content = null;
            try
            {
                content = JsonConvert.DeserializeObject<User[]>(response.Content);
            }
            catch
            {
                
            }
            return content;
        }

        public User GetUser(int userId)
        {
            var resource = $"users/{userId}";
            var request = new RestRequest(resource, Method.GET);
            
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => !r.IsSuccessful)
                .Retry(retryMaxCount, 
                    (result, i) => Log.Information($"{i} retry of GET to /{resource}"));
            var response = retryPolicy.Execute(() => _restClient.Execute(request));
            
            User content = null;
            try
            {
                content = JsonConvert.DeserializeObject<User>(response.Content);
            }
            catch
            {
                
            }
            return content;
        }

        public Post GetPost(int postId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Post> GetPosts()
        {
            throw new NotImplementedException();
        }

        public int Comment(CommentContent commentContent, int postId, string token)
        {
            throw new NotImplementedException();
        }

        public int Reply(ReplyContent replyContent, int postId, int commentId, string token)
        {
            throw new NotImplementedException();
        }

        public int Post(PostContent postContent, int userId, string token)
        {
            var resource = $"users/{userId}/posts";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(postContent);
            request.AddHeader("Authorization", token);
            
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(r => !r.IsSuccessful)
                .Retry(retryMaxCount, 
                    (result, i) => Log.Information($"{i} retry of POST to /{resource}"));
            var response = retryPolicy.Execute(() => _restClient.Execute<int>(request));
            var content = 0;
            try
            {
                content = JsonConvert.DeserializeObject<int>(response.Content);
            }
            catch (Exception e)
            {
                Log.Error($"Retry count ({retryMaxCount}) on POST by userID {userId} exceeded");
            }
            return content;
        }

        public void GET<T>()
        {
            
        }
        public void POST<T>()
        {
            
        }
        
        private readonly RestClient _restClient;
        private readonly int retryMaxCount;
    }
}