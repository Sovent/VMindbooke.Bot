using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using VMindBooke.SDK.Domain;

namespace VMindBooke.SDK.Application
{
    public class UserService : IUserService
    {
        private RestClient _restClient;
        private Policy<IRestResponse> _retryPolicy;

        public UserService(RestClient restClient)
        {
            _restClient = restClient;
            _retryPolicy = Policy<IRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetry(15, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)));
        }

        public IEnumerable<User> GetAllUsers()
        {
            var request = new RestRequest("users", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<List<User>>(response.Content);
            return content ?? new List<User>();
        }

        public User GetUser(int id)
        {
            var request = new RestRequest($"users/{id}", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<User>(response.Content);
            return content;
        }

        public IEnumerable<Post> GetUserPosts(User user)
        {
            var request = new RestRequest($"users/{user.Id}/posts", Method.GET);
            var response = _retryPolicy.Execute(() => _restClient.Execute(request));
            var content = JsonConvert.DeserializeObject<List<Post>>(response.Content);
            return content ?? new List<Post>();
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

        public void CreatePost(User actor, string title, string content)
        {
            var request = new RestRequest($"users/{actor.Id}/posts", Method.POST);
            request.AddHeader("Authorization", actor.Token);

            request.AddJsonBody(
                new
                {
                    title, content,
                });
            _retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public User GetAuthorizedUser(int id, string token)
        {
            var rawUser = GetUser(id);
            var authorizedUser = new User(
                rawUser.Id,
                token,
                rawUser.Name,
                rawUser.Likes);
            return authorizedUser;
        }

        public User UserLikesUpdate(User user)
        {
            var rawUser = GetUser(user.Id);
            var authorizedUser = new User(
                user.Id,
                user.Token,
                user.Name,
                rawUser.Likes);
            return authorizedUser;
        }
    }
}