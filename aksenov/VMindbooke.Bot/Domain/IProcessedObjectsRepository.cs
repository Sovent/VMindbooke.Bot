namespace VMindbooke.Bot.Domain
{
    public interface IProcessedObjectsRepository
    {
        bool DoesContainCopiedPostWith(int postId);
        bool DoesContainCommentedPostWith(int postId);
        bool DoesContainRepliedCommentWith(string commentId);
        void AddNewCopiedPost(int postId);
        void AddNewCommentedPost(int postId);
        void AddNewRepliedComment(string commentId);

    }
}