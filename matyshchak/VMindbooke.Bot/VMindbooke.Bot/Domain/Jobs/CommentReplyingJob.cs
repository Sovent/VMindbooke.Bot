using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Usage.Domain.ContentProviders;
using Usage.Domain.Entities;
using Usage.Domain.ValueObjects;

namespace Usage.Domain.Jobs
{
    public class CommentReplyingJob : IBoostingJob
    {
        public CommentReplyingJob(UserCredentials userCredentials,
            IVmClient client,
            CommentLikesToReplyThreshold likesToReplyThreshold,
            ICommentContentProvider commentContentProvider)
        {
            _userCredentials = userCredentials;
            _client = client;
            _likesToReplyThreshold = likesToReplyThreshold;
            _commentContentProvider = commentContentProvider;
        }
        
        private readonly UserCredentials _userCredentials;
        private readonly IVmClient _client;
        private readonly CommentLikesToReplyThreshold _likesToReplyThreshold;
        private readonly ICommentContentProvider _commentContentProvider;
        private readonly HashSet<Guid> _repliedCommentsIds = new HashSet<Guid>();
        
        public void Execute()
        {
            Log.Information("Executing CommentReplyingJob.");
            var posts = _client.GetAllPosts();
            foreach (var post in posts)
            {
                foreach (var comment in post.Comments)
                {
                    var numberOfDailyLikes = comment.Likes.Count(like =>
                        like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);

                    if (numberOfDailyLikes < _likesToReplyThreshold.Value)
                        continue;
                    
                    if (_repliedCommentsIds.Contains(comment.Id))
                    {
                        Log.Information($"Comment with Id: {comment.Id} is already replied.");
                        continue;
                    }

                    Log.Information($"Adding reply to comment with Id: {comment.Id}.");
                    _client.ReplyToComment(_userCredentials.Token,
                        post.Id,
                        comment.Id,
                        _commentContentProvider.GetCommentContent());
                    _repliedCommentsIds.Add(comment.Id);
                }
            }
        }
    }
}