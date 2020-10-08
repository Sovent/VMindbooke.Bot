using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Polly;
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
            _restClientRequester.SendRequestParseResult<IEnumerable<User>>(Method.GET, $"/users",
                MakePaginationParams(skip, take));
        
        public User GetUser(int userId) =>
            _restClientRequester.SendRequestParseResult<User>(Method.GET, $"/users/{userId}");
        
        public IEnumerable<Post> GetUserPosts(int userId) =>
            _restClientRequester.SendRequestParseResult<IEnumerable<Post>>(Method.GET, $"/users/{userId}");
        
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
                Method.POST, "/users", newUser);
        
        public int CreatePost(UserCredentials credentials, int userId, NewPost newPost) =>
             int.Parse(_restClientRequester.SendRequest<NewPost>(
                 Method.POST, $"/users/{userId}/posts", newPost,
                 null, GetAuthHeaders(credentials)).Content);
        
        public void LikePost(UserCredentials credentials, int postId) =>
            _restClientRequester.SendRequest(
                Method.POST, $"/posts/{postId}/likes",
                null, GetAuthHeaders(credentials));
        
        public void CreateComment(UserCredentials credentials, int postId, NewComment newComment) =>
            _restClientRequester.SendRequest(
                Method.POST, $"/posts/{postId}/comments", newComment,
                null, GetAuthHeaders(credentials));
        
        public void LikeComment(UserCredentials credentials, int postId, Guid commentId) =>
            _restClientRequester.SendRequest(
                Method.POST,
                $"/users/{postId}/comments/{commentId}/likes",
                null, GetAuthHeaders(credentials));
        
        public void CreateReply(UserCredentials credentials, int postId, Guid commentId, NewComment reply) =>
            _restClientRequester.SendRequest(
                Method.POST,
                $"/users/{postId}/comments/{commentId}/replies",
                reply,
                null, 
                GetAuthHeaders(credentials));
        
        
        /*public IReadOnlyCollection<User> GetUsers()
        {
            var client = new RestClient("http://135.181.101.47");
            var request = new RestRequest("users",Method.GET);

            var retryPolicy = Policy.HandleResult<IRestResponse>
                (r=> !r.IsSuccessful)
                .Retry(3);
            
            
            var response = retryPolicy.Execute(()=> client.Execute(request));
            var content = JsonConvert.DeserializeObject<List<User>>(response.Content);
            return content;
        }

        public string RegisterUser(User user)
        {
            var client = new RestClient("http://135.181.101.47");
            var request = new RestRequest("users",Method.POST);
            request.AddJsonBody(user);
            var response = client.Execute(request);
            var content = JsonConvert.DeserializeObject<UserRegistrationResponse>(response.Content);
            return content.token;
        }

        public string AddPost(Post post,int SenderId)
        {
            var client = new RestClient("http://135.181.101.47");
            var request = new RestRequest($"users/{SenderId}/posts",Method.POST);
            request.AddJsonBody(post);
            request.AddHeader("Authorization", "7b5eba5e60ae4b238f2cf50438e26bf3");
            
            var retryPolicy = Policy.HandleResult<IRestResponse>
                    (r=> !r.IsSuccessful)
                .Retry(10);
            
            
            var response = retryPolicy.Execute(()=> client.Execute(request));
            //var content = JsonConvert.DeserializeObject<int>(response.Content);
            return response.Content;
        }

        public int LikeAll(int SenderId)
        {
            for (int i = 1; i < 100; i++)
            {
                var client = new RestClient("http://135.181.101.47");
                var request = new RestRequest($"/posts/{i}/likes",Method.POST);

                request.AddHeader("Authorization", "7b5eba5e60ae4b238f2cf50438e26bf3");

                var retryPolicy = Policy.HandleResult<IRestResponse>
                        (r => !r.IsSuccessful)
                    .Retry(10);


                var response = retryPolicy.Execute(() => client.Execute(request));
                Console.WriteLine("123");
            }

            //var content = JsonConvert.DeserializeObject<int>(response.Content);
            return 1;
        }*/
    }

    
}