using System;
using System.Linq;
using Hangfire;
using Hangfire.MemoryStorage;
using Serilog;
using Usage;
using VMindbookeBotSDK;

//TODO Using Optinal in this case probably not best idea
//TODO big class how uncouple scenarios? extract each scenarios in separate class?

namespace VMindbookeBot
{
    public class BotService
    {
        public Configuration Config { get; }
        public HttpRequester HttpRequester { get; }
        public IWordsGenerator WordsGenerator { get; }
        private readonly Policies _policies;


        //Hangfire needs parameterless constructor
        public BotService()
        {
            _policies = new Policies(this);
            Config = new Configuration("appsettings.json");
            HttpRequester = HttpRequester.Create("TestBot 1");
            WordsGenerator = Usage.WordsGenerator.Create(Config.Local);
        }

        private BotService(Configuration config, HttpRequester httpRequester, IWordsGenerator wordsGenerator)
        {
            _policies = new Policies(this);
            Config = config;
            HttpRequester = httpRequester;
            WordsGenerator = wordsGenerator;
        }

        public Policies Policies
        {
            get { return _policies; }
        }

        public static BotService Create(Configuration config)
        {
            CreateLogger(config);
            return new BotService(config, HttpRequester.Create("TestBot 1"), Usage.WordsGenerator.Create(config.Local));
        }

        private static void CreateLogger(Configuration config)
        {
            var configuration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(config.LogName)
                .WriteTo.Console();
            //.Filter.ByExcluding(Matching.FromSource("Hangfire"));
            Log.Logger = configuration.CreateLogger();
        }

        public void WriteComment(int postId)
        {
            Log.Information($"WriteComment postId={postId}");
            var threshold = Config.WriteCommentThreshold;
            var likesFromPost = GetPostLikes(postId);
            var likesCount = likesFromPost.Map(x => x.Length);
            likesCount.Do(likes =>
                {
                    if (likes >= threshold)
                    {
                        PostComment(postId, WordsGenerator.GetContent());
                    }
                }
            );
        }

        public void WriteReply(int postId)
        {
            Log.Information($"WriteReply postId={postId}");
            var threshold = Config.ReplyCommentThreshold;
            var postComments = GetPostComments(postId);
            postComments.Do(comments =>
                {
                    foreach (var comment in comments)
                    {
                        if (comment.likes.Length >= threshold)
                        {
                            PostReply(postId, comment.id, WordsGenerator.GetContent());
                        }
                    }
                }
            );
        }

        public void CopyPost(int postId)
        {
            Log.Information($"CopyPost postId={postId}");
            var threshold = Config.CopyCommentThreshold;
            var post = GetPost(postId);
            var likesFromPost = post.OptionalResult.Map(p => p.likes);
            var likesCount = likesFromPost.Map(x => x.Length);
            post.OptionalResult.Do(p =>
            {
                likesCount.Do(likes =>
                {
                    if (likes >= threshold)
                    {
                        CreatePostFromContent(HttpRequester.UserId, WordsGenerator.GetTitle(), p.content);
                    }
                });
            });
        }

        public void CopyMostLikedPost(int userId, TimeSpan timeSpan)
        {
            var pastTime = DateTime.Now - timeSpan;
            Log.Information($"CopyMostLikedPost userId={userId}, from {pastTime} to {DateTime.Now}");
            var threshold = Config.MostLikedCommentThreshold;
            var user = GetUser(userId);
            var userLikes = user.OptionalResult.Map(p => p.likes);
            var likesForPastTimeToNow = userLikes.Map(
                likes => { return likes.Count(like => like.placingDateUtc > pastTime); });
            likesForPastTimeToNow.Do(likes =>
            {
                if (likes >= threshold)
                {
                    var userPosts = GetUsersPost(userId);
                    var mostLikedUserPost = userPosts.OptionalResult
                        .Map(posts => posts.OrderByDescending(post => post.likes.Length).First());
                    mostLikedUserPost.Do(post =>
                        CreatePostFromContent(HttpRequester.UserId, post.title, post.content));
                }
            });
        }

        public void Boost(int userId, Action endingEvent)
        {
            try
            {
                GlobalConfiguration.Configuration.UseMemoryStorage();
                Log.Information($"Start Boost userId={userId}");
                var threshold = Config.StopBoostThreshold;
                // RecurringJob.AddOrUpdate<BotService>(client => StartDailyBoost(userId, threshold), Cron.Daily);
                BackgroundJob.Enqueue<BoostJob>(client => new BoostJob().StartDailyBoost(userId, threshold, this));
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Wow, something go wrong");
            }

            using (var backgroundServer = new BackgroundJobServer())
            {
                Log.Information("Background service started");
                endingEvent();
            }
        }

        private Optional<Like[]> GetPostLikes(int postId)
        {
            var post = GetPost(postId);
            return post.OptionalResult.Map(p => p.likes);
        }

        private Optional<Comment[]> GetPostComments(int postId)
        {
            var post = GetPost(postId);
            return post.OptionalResult.Map(p => p.comments);
        }

        private Result<Post, HttpRequester.StatusCode> GetPost(int postId)
        {
            var getPostPolicy = Policies.GetPostPolicy(postId);
            var post = getPostPolicy.Execute(() => HttpRequester.GetPost(postId));
            return post;
        }

        private Result<Post[], HttpRequester.StatusCode> GetUsersPost(int userId)
        {
            var getPostPolicy = Policies.GetUsersPostPolicy(userId);
            var userPosts = getPostPolicy.Execute(() => HttpRequester.GetUserPosts(userId));
            return userPosts;
        }

        internal Result<UserInfoByUserId, HttpRequester.StatusCode> GetUser(int userId)
        {
            var getPostPolicy = Policies.GetUserPolicy(userId);
            var post = getPostPolicy.Execute(() => HttpRequester.GetUserInfo(userId));
            return post;
        }

        private void CreatePostFromContent(int userId, string title, string content)
        {
            var postReplyPolicy = Policies.GetCreateNewPostPolicy(userId);
            var result = postReplyPolicy.Execute(() => HttpRequester.WriteNewPost(userId, title, content));
            var postId = result.OptionalResult;
            postId.Do(id =>
            {
                Log.Information($"New Post Created postId={id}");
                Log.Verbose($"New Post Created postId={id} {nameof(title)}={title} {nameof(content)}={content}");
            });
        }

        private void PostReply(int postId, string commentId, string content)
        {
            var postReplyPolicy = Policies.GetPostReplyPolicy(postId, commentId);
            postReplyPolicy.Execute(() => HttpRequester.WriteReply(postId, commentId, content));
            Log.Information($"Reply left {nameof(postId)}={postId} {nameof(commentId)}={commentId}");
            Log.Verbose(
                $"Reply left {nameof(postId)}={postId} {nameof(commentId)}={commentId} {nameof(content)}={content} ");
        }

        private void PostComment(int postId, string content)
        {
            var writeCommentPolicy = Policies.GetWriteCommentPolicy(postId);
            writeCommentPolicy.Execute(() => HttpRequester.WriteComment(postId, WordsGenerator.GetContent()));
            Log.Information($"Comment posted {nameof(postId)}={postId}");
            Log.Verbose($"Comment posted {nameof(postId)}={postId} {nameof(content)}={content}");
        }

       
    }
}