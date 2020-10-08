using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Application
{
    public class LikeBoostingService
    {
        private readonly BotSettings _settings;
        private DateTime _currentDate;
        
        public bool IsRunning { get; private set; }

        public LikeBoostingService(BotSettings settings)
        {
            _settings = settings;
            IsRunning = true;
            _currentDate = DateTime.Now;
        }

        public void CommentsWritingScenario()
        {
            if (!IsRunning) return;
            
            var unprocessedPosts = GetUnprocessedPosts();

            foreach (var post in unprocessedPosts)
            {
                if (post.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                    _settings.LikeLimitForPostToMakeComment)
                {
                    PostAComment(post.Id);
                    throw new NotImplementedException("save data, logger");
                }
            }
        }

        public void RepliesWritingScenario()
        {
            if (!IsRunning) return;
            
            var unprocessedPosts = GetUnprocessedPosts();

            foreach (var post in unprocessedPosts)
            {
                foreach (var comment in post.Comments)
                {
                    if (comment.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                        _settings.LikeLimitForCommentToMakeReply)
                    {
                        ReplyToComment(post.Id, comment.Id);
                        throw new NotImplementedException("save data, logger");
                    }
                }
            }
        }

        public void PostsCopyingByLikesScenario()
        {
            if (!IsRunning) return;
            
            var unprocessedPosts = GetUnprocessedPosts();

            foreach (var post in unprocessedPosts)
            {
                if (post.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                    _settings.LikeLimitForPostToCopy)
                {
                    CopyThePost(post);
                    throw new NotImplementedException("save data, logger");
                }
            }
        }

        public void PostsCopyingByUsersScenario()
        {
            if (!IsRunning) return;
            
            var unprocessedUsers = GetUnprocessedUsers();

            foreach (var user in unprocessedUsers)
            {
                if (user.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                    _settings.LikeLimitForUserToCopyPost)
                {
                    CopyTheMostLikedPost(user.Id);
                    throw new NotImplementedException("save data, logger");
                }
            }
        }

        public void BoostFinishScenario()
        {
            if (!IsRunning)
            {
                if (_currentDate.Date != DateTime.Now.Date)
                {
                    _currentDate = DateTime.Now;
                    IsRunning = true;
                }

                return;
            }
            
            var user = GetUserBy(_settings.UserId);

            if (user.Likes.Count(like => like.PlacingDateUtc.Date == DateTime.Now.Date) >=
                _settings.LikeLimitToCompleteProcess)
            {
                IsRunning = false;
                throw new NotImplementedException("logger");
            }
        }

        public IEnumerable<Post> GetUnprocessedPosts()
        {
            var client = new RestClient(_settings.ServerAddress);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .Retry(5);
            var request = new RestRequest("posts", Method.GET);

            var serverResponse = retryPolicy.Execute(() => client.Execute(request));
            var content = JsonConvert.DeserializeObject<Post[]>(serverResponse.Content);

            throw new NotImplementedException("check / save hash");

            return content;
        }

        public IEnumerable<User> GetUnprocessedUsers()
        {
            var client = new RestClient(_settings.ServerAddress);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .Retry(5);
            var request = new RestRequest("users", Method.GET);

            var serverResponse = retryPolicy.Execute(() => client.Execute(request));
            var content = JsonConvert.DeserializeObject<User[]>(serverResponse.Content);

            throw new NotImplementedException("check / save hash");

            return content;
        }

        public void PostAComment(int postId)
        {
            var client = new RestClient(_settings.ServerAddress);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)
                });

            var resource = $"posts/{postId}/comments";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new CommentRequest("Hey there. Nice post!"));
            request.AddHeader("Authorization", _settings.UserToken);

            var serverResponse = retryPolicy.Execute(() => client.Execute(request));

            if (!serverResponse.IsSuccessful)
            {
                throw new NotImplementedException("logger");
            }
        }

        public void ReplyToComment(int postId, string commentId)
        {
            var client = new RestClient(_settings.ServerAddress);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)
                });

            var resource = $"posts/{postId}/comments/{commentId}/replies";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new CommentRequest("Hey there. Nice comment!"));
            request.AddHeader("Authorization", _settings.UserToken);

            var serverResponse = retryPolicy.Execute(() => client.Execute(request));

            if (!serverResponse.IsSuccessful)
            {
                throw new NotImplementedException("logger");
            }
        }

        public void CopyThePost(Post post)
        {
            var client = new RestClient(_settings.ServerAddress);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5)
                });

            var resource = $"users/{_settings.UserId}/posts";
            var postTitle = CreateNewTitleFromContent(post.Content);
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new PostRequest(postTitle, post.Content));
            request.AddHeader("Authorization", _settings.UserToken);

            var serverResponse = retryPolicy.Execute(() => client.Execute(request));

            if (!serverResponse.IsSuccessful)
            {
                throw new NotImplementedException("logger");
            }
        }

        public string CreateNewTitleFromContent(string postContent)
        {
            string title;
            
            if (postContent.Split(" ").Length < 3)
            {
                title = "New interesting post";
            }
            else
            {
                var contentInList = postContent.Split(" ").Take(3).ToList();
                title = $"{contentInList[0]} {contentInList[1]} {contentInList[2]}...";
            }

            return title;
        }

        public void CopyTheMostLikedPost(int userId)
        {
            var theMostLikedUserPost = GetTheMostLikedUserPost(userId);
            
            var client = new RestClient(_settings.ServerAddress);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5)
                });

            var resource = $"users/{_settings.UserId}/posts";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(new PostRequest(theMostLikedUserPost.Title, theMostLikedUserPost.Content));
            request.AddHeader("Authorization", _settings.UserToken);

            var serverResponse = retryPolicy.Execute(() => client.Execute(request));

            if (!serverResponse.IsSuccessful)
            {
                throw new NotImplementedException("logger");
            }
        }

        public Post GetTheMostLikedUserPost(int userId)
        {
            var posts = GetUserPosts(userId);

            return posts
                .OrderByDescending(post => post.Likes.Length)
                .FirstOrDefault();
        }

        public IEnumerable<Post> GetUserPosts(int userId)
        {
            var client = new RestClient(_settings.ServerAddress);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .Retry(5);
            var resource = $"users/{userId}/posts";
            var request = new RestRequest(resource, Method.GET);

            var serverResponse = retryPolicy.Execute(() => client.Execute(request));
            var content = JsonConvert.DeserializeObject<Post[]>(serverResponse.Content);

            throw new NotImplementedException("logger");

            return content;
        }

        public User GetUserBy(int userId)
        {
            var client = new RestClient(_settings.ServerAddress);
            var retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .Retry(5);
            var resource = $"users/{userId}";
            var request = new RestRequest(resource, Method.GET);

            var serverResponse = retryPolicy.Execute(() => client.Execute(request));
            var content = JsonConvert.DeserializeObject<User>(serverResponse.Content);

            throw new NotImplementedException("logger");

            return content;
        }
    }
}