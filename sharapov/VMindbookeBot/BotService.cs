using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Primitives;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using VMindbookeBot;
using VMindbookeBotSDK;

namespace Usage
{
    public class BotService
    {
        private readonly Configuration _config;
        private readonly HttpRequester _httpRequester;
        private readonly IWordsGenerator _wordsGenerator;

        private BotService(Configuration config, HttpRequester httpRequester, IWordsGenerator wordsGenerator)
        {
            _config = config;
            _httpRequester = httpRequester;
            _wordsGenerator = wordsGenerator;
        }

        public static BotService Create(Configuration config)
        {
            CreateLogger(config);
            return new BotService(config, HttpRequester.Create("TestBot 1"), WordsGenerator.Create(config.Local));
        }

        private static void CreateLogger(Configuration config)
        {
            var configuration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(config.LogName)
                .WriteTo.Console();
            Log.Logger = configuration.CreateLogger();
        }

        public void WriteComment(int postId)
        {
            var threshold = _config.WriteCommentThreshold;
            var likesFromPost = GetPostLikes(postId);
            var likesCount = likesFromPost.Map(x => x.Length);
            likesCount.Do(likes =>
                {
                    if (likes >= threshold)
                    {
                        PostComment(postId, _wordsGenerator.GetContent());
                    }
                }
            );
        }

        public void WriteReply(int postId)
        {
            var threshold = _config.ReplyCommentThreshold;
            var postComments = GetPostComments(postId);
            postComments.Do(comments =>
                {
                    foreach (var comment in comments)
                    {
                        if (comment.likes.Length >= threshold)
                        {
                            PostReply(postId, comment.id, _wordsGenerator.GetContent());
                        }
                    }
                }
            );   
        }

        public void CopyPost(int postId)
        {
            var threshold = _config.CopyCommentThreshold;
            var post = GetPost(postId);
            var likesFromPost = post.OptionalResult.Map(p => p.likes);
            var likesCount = likesFromPost.Map(x => x.Length);
            post.OptionalResult.Do(p =>
            {
                likesCount.Do(likes =>
                {
                    if (likes >= threshold)
                    {
                        CreatePostFromContent(_httpRequester.UserId, _wordsGenerator.GetTitle(), p.content);
                    }
                });
            });
        }

        private void CreatePostFromContent(int userId, string title, string content)
        {
            var postReplyPolicy = GetCreateNewPostPolicy(userId);
            var result = postReplyPolicy.Execute(() => _httpRequester.WriteNewPost(userId, title, content));
            var postId = result.OptionalResult;
            postId.Do(id => Log.Information($"New Post Created {nameof(postId)}={postId}"));
            Log.Verbose($"{nameof(title)}={title} {nameof(content)}={content}");
        }

        private void PostReply(int postId, string commentId, string content)
        {
            var postReplyPolicy = GetPostReplyPolicy(postId, commentId);
            postReplyPolicy.Execute(() => _httpRequester.WriteReply(postId, commentId, content));
            Log.Information($"Reply to {nameof(postId)}={postId} {nameof(commentId)}={commentId} left");
            Log.Verbose($"{nameof(content)}={content} ");
        }
        
        private void PostComment(int postId, string content)
        {
            var writeCommentPolicy = GetWriteCommentPolicy(postId);
            writeCommentPolicy.Execute(() => _httpRequester.WriteComment(postId, _wordsGenerator.GetContent()));
            Log.Information($"Comment to {nameof(postId)}={postId} posted");
            Log.Verbose($"{nameof(content)}={content}");
        }
        
        private RetryPolicy<Result<int, HttpRequester.StatusCode>> GetCreateNewPostPolicy(int userId)
        {
            return Policy<Result<int, HttpRequester.StatusCode>>
                .HandleResult(r => !r.HasValue)
                .WaitAndRetry(_config.Retry, PolynomiallyTimeSpan,
                    (r, span) =>
                        Log.Warning(
                            $"Retry Create New Post {nameof(userId)}={userId} : Time span {span}"));
        }
        
        private RetryPolicy<HttpRequester.StatusCode> GetWriteCommentPolicy(int postId)
        {
            return Policy<HttpRequester.StatusCode>
                .HandleResult(p => p.IsPostError())
                .WaitAndRetry(_config.Retry, PolynomiallyTimeSpan,
                    (r, span) =>
                        Log.Warning(
                            $"Retry Write Comment {nameof(postId)}={postId} : Time span {span}"));
        }

        private Optional<Like[]> GetPostLikes(int postId)
        {
            var post = GetPost(postId);
            return post.OptionalResult.Map(p => p.likes);
        }
        
        private Optional<Comment[]> GetPostComments(int postId)
        {
            var post = GetPost(postId);
            return post.OptionalResult.Map(p=> p.comments);
        }

        private Result<Post, HttpRequester.StatusCode> GetPost(int postId)
        {
            var getPostPolicy = GetPostPolicy(postId);
            var post = getPostPolicy.Execute(() => _httpRequester.GetPost(postId));
            return post;
        }

        private RetryPolicy<Result<Post, HttpRequester.StatusCode>> GetPostPolicy(int postId)
        {
            var getPostPolicy = Policy<Result<Post, HttpRequester.StatusCode>>
                .HandleResult(r => !r.HasValue)
                .WaitAndRetry(_config.Retry, PolynomiallyTimeSpan,
                    (r, span)
                        => Log.Warning(
                            $"Retry Get Post {nameof(postId)}={postId} : Time span {span}"
                        ));
            return getPostPolicy;
        }
        
        
        private RetryPolicy<HttpRequester.StatusCode> GetPostReplyPolicy(int postId, string commentId)
        {
            var getPostPolicy = Policy<HttpRequester.StatusCode>
                .HandleResult(p => p.IsPostError())
                .WaitAndRetry(_config.Retry, PolynomiallyTimeSpan,
                    (r, span)
                        => Log.Warning(
                            $"Retry Post Reply {nameof(postId)}={postId} {nameof(commentId)}={commentId} : Time span {span}"
                        ));
            return getPostPolicy;
        }


        private static TimeSpan PolynomiallyTimeSpan(int retryAttempt)
        {
            return TimeSpan.FromSeconds(1); 
            // return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)); //TODO Sometimes awaiting is too long  
        }
    }
}