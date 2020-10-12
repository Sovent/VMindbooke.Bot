namespace Usage.Domain.ValueObjects.LikeThresholds
{
    public class PostLikesToStealThreshold
    {
        public PostLikesToStealThreshold(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}