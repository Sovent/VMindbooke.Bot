namespace Usage.Domain.ValueObjects
{
    public class CommentLikesToReplyThreshold
    {
        public CommentLikesToReplyThreshold(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}