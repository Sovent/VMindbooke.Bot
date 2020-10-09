using System;
using System.Collections.Generic;
using System.Linq;
using VMBookeBot.Domain;
using VMBook.SDK;

namespace VMBookTests
{
    public class TestsRequestRetryer : IRequestRetryer
    {
        private List<Post> _posts;
        public TestsRequestRetryer(RepositoryForTests repository)
        {
            _posts = repository.GetPosts();
        }
        public bool TryWriteComment(int postId, int retryCount)
        {
            if (_posts.All(post => post.Id != postId)) return false;
            var postToComment = _posts.First(post => post.Id == postId);
            postToComment.Comments.Add(new Comment());
            return true;
        }

        public bool TryWriteReplyUnderComment(int postId, Guid commentId, int retryCount)
        {
            throw new NotImplementedException();
        }

        public bool TryMakeCopyOfPost(string postTitle, string postContent, int originalPostId, int retryCount)
        {
            throw new NotImplementedException();
        }
    }
}