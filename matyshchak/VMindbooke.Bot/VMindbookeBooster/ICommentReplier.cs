using System;
using System.Collections.Generic;
using System.Linq;
using VMindbookeBooster.Entities;

namespace VMindbookeBooster
{
    public interface ICommentReplier
    {
        void ReplyToComments(int likesThreshold, CommentContent reply);
    }

    public class CommentReplier : ICommentReplier
    {
        public CommentReplier(UserCredentials userCredentials, IVmClient client)
        {
            _userCredentials = userCredentials;
            _client = client;
        }
        
        private readonly UserCredentials _userCredentials;
        private readonly IVmClient _client;
        private readonly HashSet<Guid> _repliedCommentsIds = new HashSet<Guid>();
        
        public void ReplyToComments(int likesThreshold, CommentContent reply)
        {
            var comments = _client.GetAllComments();
            var posts = _client.GetAllPosts();
            foreach (var post in posts)
            {
                foreach (var comment in post.Comments)
                {
                    var numberOfDailyLikes = comment.Likes.Count(like =>
                        like.PlacingDateUtc.Day == DateTime.Now.ToUniversalTime().Day);

                    if (numberOfDailyLikes < likesThreshold)
                        continue;
                    if (_repliedCommentsIds.Contains(comment.Id))
                    {
                        Console.WriteLine($"comment {comment.Id} is already replied");
                        continue;
                    }

                    Console.WriteLine($"Added reply to post with id {comment.Id}");
                    _client.ReplyToComment(_userCredentials.Token, post.Id, comment.Id, reply);
                    _repliedCommentsIds.Add(comment.Id);
                }
            }
        }
    }
}