using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using VMindbookeBot;

namespace VMindbookeBotSDK
{
    public class HttpRequester
    {
        public string AuthToken { get; }
        
        public int UserId { get; }

        private readonly IRestClient _restClient;
        

        public HttpRequester(int userId, string authToken, IRestClient restClient)
        {
            AuthToken = authToken;
            _restClient = restClient;
            UserId = userId;
        }

        private static string TimeFormat => "yyyy-MM-ddTHH:mm:sssssssZ";

        public static HttpRequester Create(string userName)
        {
            var restClient = new RestClient("http://135.181.101.47");
            var request = new RestRequest("users", Method.POST);
            request.AddJsonBody(new {name = userName});
            var response = restClient.Execute(request);
            var user = JsonConvert.DeserializeObject<User>(response.Content);
            return new HttpRequester(user.id, user.token, restClient);
        }

        public Result<Post, StatusCode> GetPost(int postId)
        {
            var request = CreateRequestWithAuthorizationHeader($"/posts/{postId}", Method.GET);
            var response = _restClient.Execute(request);
            if (response.StatusCode == ErrorCode())
            {
                return Result<Post, StatusCode>.None(StatusCode.InternalServerError);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result<Post, StatusCode>.None(StatusCode.NotFound);
            }
            
            var userPost = JsonConvert
                .DeserializeObject<Post>(response.Content,
                    new IsoDateTimeConverter {DateTimeFormat = TimeFormat});
            return userPost == null
                ? Result<Post, StatusCode>.None(StatusCode.DeserializeObjectError)
                : Result<Post, StatusCode>.Some(userPost, StatusCode.Ok);
        }

        public StatusCode WriteComment(int postId, string contentText)
        {
            var request = CreateRequestWithAuthorizationHeader($"/posts/{postId}/comments", Method.POST);
            request.AddJsonBody(new {content = contentText});
            var response = _restClient.Execute(request);
            return response.StatusCode == ErrorCode() ? StatusCode.InternalServerError : StatusCode.Ok;
        }

        public StatusCode WriteReply(int postId, string commentId, string contentText)
        {
            var request =
                CreateRequestWithAuthorizationHeader($"/posts/{postId}/comments/{commentId}/replies", Method.POST);
            request.AddJsonBody(new {content = contentText});
            var response = _restClient.Execute(request);
            return response.StatusCode == ErrorCode() ? StatusCode.InternalServerError : StatusCode.Ok;
        }

        public Result<int, StatusCode> WriteNewPost(int userId, string titleText, string contentText)
        {
            var request = CreateRequestWithAuthorizationHeader($"/users/{userId}/posts", Method.POST);
            request.AddJsonBody(new {title = titleText, content = contentText});
            var response = _restClient.Execute(request);
            if (response.StatusCode == ErrorCode())
            {
                return Result<int, StatusCode>.None(StatusCode.InternalServerError);
            }
            var newPostId = Convert.ToInt32(response.Content);
            return response.StatusCode == ErrorCode() 
                ? Result<int, StatusCode>.None(StatusCode.InternalServerError) 
                : Result<int, StatusCode>.Some(newPostId, StatusCode.Ok);
        }

        public Result<IEnumerable<Like>, StatusCode> GetUserInfo(int userId)
        {
            var request = CreateRequestWithAuthorizationHeader($"/users/{userId}", Method.GET);
            var response = _restClient.Execute(request);
            if (response.StatusCode == ErrorCode())
            {
                return Result<IEnumerable<Like>, StatusCode>.None(StatusCode.InternalServerError);
            }

            var userInfo = JsonConvert
                .DeserializeObject<UserInfoByUserId>(response.Content,
                    new IsoDateTimeConverter {DateTimeFormat = TimeFormat});
            return Result<IEnumerable<Like>, StatusCode>.Some(userInfo?.likes, StatusCode.InternalServerError);
        }

        private static HttpStatusCode ErrorCode()
        {
            return HttpStatusCode.InternalServerError;
        }
        
        

        private IRestRequest CreateRequestWithAuthorizationHeader(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            return request.AddHeader("Authorization", AuthToken);
        }

        public enum StatusCode
        {
            Ok,
            Error,
            DeserializeObjectError,
            InternalServerError,
            NotFound
        }
    }
}