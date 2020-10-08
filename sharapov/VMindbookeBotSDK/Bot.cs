using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;

namespace VMindbookeBot
{
    public class Bot
    {
        public string AuthToken { get; }

        public Bot(string authToken)
        {
            AuthToken = authToken;
        }

        private static string TimeFormat => "yyyy-MM-ddTHH:mm:sssssssZ"; //TODO to another class

        public static Bot Create(string userName)
        {
            var restClient = new RestClient("http://135.181.101.47");
            var request = new RestRequest($"users", Method.POST);
            request.AddJsonBody(new {name = userName});
            var response = restClient.Execute(request);
            var user = JsonConvert.DeserializeObject<User>(response.Content);
            return new Bot(user.token);
        }

        //получить лайки по ид
        public Result<IEnumerable<Like>, StatusCode> GetLikesByPostId(int postId)
        {
            var restClient = new RestClient("http://135.181.101.47");
            var request = CreateRequestWithAuthorizationHeader($"/posts/{postId}", Method.GET);
            var response = restClient.Execute(request);
            if (response.StatusCode == ErrorCode())
            {
                return Result<IEnumerable<Like>, StatusCode>.None(StatusCode.InternalServerError);
            }
            var userPosts = JsonConvert
                .DeserializeObject<Post>(response.Content,
                    new IsoDateTimeConverter {DateTimeFormat = TimeFormat});
            return userPosts == null
                ? Result<IEnumerable<Like>, StatusCode>.None(StatusCode.DeserializeObjectError)
                : Result<IEnumerable<Like>, StatusCode>.Some(userPosts.likes, StatusCode.OK);
        }

        public StatusCode WriteComment(int postId, string contentText)
        {
            var restClient = new RestClient("http://135.181.101.47");
            var request = CreateRequestWithAuthorizationHeader($"/posts/{postId}/comments", Method.POST);
            request.AddJsonBody(new {content = contentText});
            var response = restClient.Execute(request);
            return response.StatusCode == ErrorCode() ? StatusCode.InternalServerError : StatusCode.OK;
        }

        //написать реплай к посту и коменту
        public StatusCode WriteReply(int postId, string commentId, string contentText)
        {
            var restClient = new RestClient("http://135.181.101.47");
            var request =
                CreateRequestWithAuthorizationHeader($"/posts/{postId}/comments/{commentId}/replies", Method.POST);
            request.AddJsonBody(new {content = contentText});
            var response = restClient.Execute(request);
            return response.StatusCode == ErrorCode() ? StatusCode.Error : StatusCode.OK;
        }


        //написать пост юзеру
        public StatusCode WritePost(int userId, string titleText, string contentText)
        {
            var restClient = new RestClient("http://135.181.101.47");
            var request = CreateRequestWithAuthorizationHeader($"/users/{userId}/posts", Method.POST);
            request.AddJsonBody(new {title = titleText, content = contentText});
            var response = restClient.Execute(request);
            return response.StatusCode == ErrorCode() ? StatusCode.InternalServerError : StatusCode.OK;
        }

        //вернуть лайки пользователя 
        public Result<IEnumerable<Like>, StatusCode> GetUserInfo(int userId)
        {
            var restClient = new RestClient("http://135.181.101.47");
            var request = CreateRequestWithAuthorizationHeader($"/users/{userId}", Method.GET);
            var response = restClient.Execute(request);
            if (response.StatusCode == ErrorCode())
            {
                return Result<IEnumerable<Like>, StatusCode>.None(StatusCode.Error);
            }

            var userInfo = JsonConvert
                .DeserializeObject<UserInfoByUserId>(response.Content,
                    new IsoDateTimeConverter {DateTimeFormat = TimeFormat});
            return Result<IEnumerable<Like>, StatusCode>.Some(userInfo?.likes, StatusCode.Error);
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
            OK,
            Error,
            DeserializeObjectError,
            InternalServerError
        }
    }


    public static class StatusCodeExtensions
    {
        public static bool IsWriteError(this Bot.StatusCode error)
        {
            return error == Bot.StatusCode.InternalServerError;
        }
    }
}