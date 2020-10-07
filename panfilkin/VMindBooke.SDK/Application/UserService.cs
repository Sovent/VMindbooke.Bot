using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using VMindBooke.SDK.Domain;

namespace VMindBooke.SDK.Application
{
    public class UserService : IUserService
    {
        private RestClient _restClient;
        private Policy<IRestResponse> _retryPolicy;
        
        // Set retry pow to 2 before prod!
        public UserService(RestClient restClient)
        {
            _restClient = restClient;
            _retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(5, retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)) );
        }
        
        public IReadOnlyCollection<User> GetAllUsers()
        {
            var request = new RestRequest("users", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<IReadOnlyCollection<User>>(response.Content);
            return content;
        }
        
        public User GetUser(int id)
        {
            var request = new RestRequest($"users/{id}", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<User>(response.Content);
            return content;
        }

        public IReadOnlyCollection<Post> GetUserPosts(User user)
        {
            var request = new RestRequest($"users/{user.Id}/posts", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<IReadOnlyCollection<Post>>(response.Content);
            return content;
        }

        public User CreateUser(string username)
        {
            var request = new RestRequest("users", Method.POST);
            request.AddJsonBody(
                new 
                {
                    name = username
                });
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<User>(response.Content);
            return content;
        }

        public void CreatePost(User user, string title, string content)
        {
            var request = new RestRequest($"users/{user.Id}/posts", Method.POST);
            request.AddJsonBody(
                new 
                {
                    title = title,
                    content = content,
                });
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public User GetAuthorizedUser(int id, string token)
        {
            var rawUser = GetUser(id);
            var authorizedUser = new User(
                rawUser.Id,
                token,
                rawUser.Name,
                rawUser.LikesList);
            return authorizedUser;
        }

        public User UserLikesUpdate(User user)
        {
            var rawUser = GetUser(user.Id);
            var authorizedUser = new User(
                user.Id,
                user.Token,
                user.Name,
                rawUser.LikesList);
            return authorizedUser;
        }
    }
}