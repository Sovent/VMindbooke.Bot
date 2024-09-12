using Microsoft.Extensions.Configuration;

namespace VMindbooke.Bot.Domain
{
    public class PostsProcessor
    {
        public PostsProcessor(IConfigurationRoot configuration)
        {
            PostForCommentLikeThreshold = int.Parse(configuration["PostForCommentLikeThreshold"]);
            CommentToReplyLikeThreshold = int.Parse(configuration["CommentToReplyLikeThreshold"]);
            PostToRepostLikeThreshold = int.Parse(configuration["PostToRepostLikeThreshold"]);
            PostToCopyLikeThreshold = int.Parse(configuration["PostToCopyLikeThreshold"]);
            UserToStopLikeThreshold = int.Parse(configuration["UserToStopLikeThreshold"]);
        }

        private int PostForCommentLikeThreshold { get; }
        private int CommentToReplyLikeThreshold { get; }
        private int PostToRepostLikeThreshold { get; }
        private int PostToCopyLikeThreshold { get; }
        private int UserToStopLikeThreshold { get; }

        public bool IsPostCommentable(Post post)
        {
            return post.Likes.Count > PostForCommentLikeThreshold;
        }

        public bool IsCommentReplyable(Comment comment)
        {
            return comment.Likes.Count > CommentToReplyLikeThreshold;
        }

        public bool IsPostRepostable(Post post)
        {
            return post.Likes.Count > PostToRepostLikeThreshold;
        }

        public bool IsPostCopyable(Post post)
        {
            return post.Likes.Count > PostToCopyLikeThreshold;
        }

        public bool IsUserLikedEnough(User user)
        {
            return user.Likes.Count < UserToStopLikeThreshold;
        }
    }
}