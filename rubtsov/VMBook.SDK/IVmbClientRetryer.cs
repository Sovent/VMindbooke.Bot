using System;
using RestSharp;

namespace VMBook.SDK
{
    public interface IVmbClientRetryer
    {
        public IRestResponse WriteComment(int postId);
        public IRestResponse WriteReplyUnderComment(int postId, Guid commentId);
        public IRestResponse MakeCopyOfPost(string postTitle, string postContent);
    }
}