using Usage.Domain.Entities;

namespace Usage.Domain
{
    public interface ICommentContentProvider
    {
        CommentContent GetComment();
    }
}