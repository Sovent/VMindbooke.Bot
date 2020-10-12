namespace Usage.Domain.ValueObjects.LikeThresholds
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