using System.Collections.Generic;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Infrastructure
{
    public class ProcessedObjectsRepository: IProcessedObjectsRepository
    {
        private readonly List<int> _copiedPosts = new List<int>();
        private readonly List<int> _commentedPosts = new List<int>();
        private readonly List<string> _repliedComments = new List<string>();
        
        public bool DoesContainCopiedPostWith(int postId)
        {
            return _copiedPosts.Contains(postId);
        }

        public bool DoesContainCommentedPostWith(int postId)
        {
            return _commentedPosts.Contains(postId);
        }

        public bool DoesContainRepliedCommentWith(string commentId)
        {
            return _repliedComments.Contains(commentId);
        }

        public void AddNewCopiedPost(int postId)
        {
            if (!_copiedPosts.Contains(postId))
                _copiedPosts.Add(postId);
        }

        public void AddNewCommentedPost(int postId)
        {
            if (!_commentedPosts.Contains(postId))
                _commentedPosts.Add(postId);
        }

        public void AddNewRepliedComment(string commentId)
        {
            if (!_repliedComments.Contains(commentId))
                _repliedComments.Add(commentId);
        }
    }
}