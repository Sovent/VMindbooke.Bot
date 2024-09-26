using Usage.Domain.ValueObjects;

namespace Usage.Domain.ContentProviders
{
    public interface ICommentContentProvider
    {
        CommentContent GetCommentContent();
    }
}