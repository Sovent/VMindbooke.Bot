using System.Collections.Generic;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.App
{
    public interface IVMindbookeBotService
    {
        void Start();

        // check for posts
        IReadOnlyCollection<Post> GetPosts();

        IEnumerable<Comment> GetComments(Post post);

        // if post has more than PostForCommentLikeThreshold likes, leave a comment
        void CommentIfLiked(Post post);

        // if some comment has more than CommentToReplyLikeThreshold likes, leave a reply
        void ReplyIfLiked(Post post, Comment comment);

        // if some post has more than PostToRepostLikeThreshold likes, post same content with different title
        void RepostIfLiked(Post post);

        // if some post of some user has more than PostToCopyLikeThreshold likes, post the same post
        void CopyIfLiked(Post post);

        // if i have more than UserToStopLikeThreshold likes, stop for now
        bool IsLikedEnough();
    }
}