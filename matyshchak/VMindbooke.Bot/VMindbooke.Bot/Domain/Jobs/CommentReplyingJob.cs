using System;
using System.Collections.Generic;
using System.Linq;
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
            var comments = _client.GetAllComments();
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
                        Console.WriteLine($"comment {comment.Id} is already replied");
                        continue;
                    }

                    Console.WriteLine($"Added reply to post with id {comment.Id}");
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