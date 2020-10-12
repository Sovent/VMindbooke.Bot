namespace Usage.Domain.ValueObjects.LikeThresholds
{
    public class PostLikesToCommentThreshold
    {
        public PostLikesToCommentThreshold(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}