namespace Usage.Domain.ValueObjects
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