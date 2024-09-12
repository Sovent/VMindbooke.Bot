namespace VMindbooke.Bot.Domain
{
    public interface IActionMaker
    {
        void CommentPost(Post post);

        void ReplyToComment(Post post, Comment comment);

        void RepostPost(Post post);

        void CopyPost(Post post);
    }
}