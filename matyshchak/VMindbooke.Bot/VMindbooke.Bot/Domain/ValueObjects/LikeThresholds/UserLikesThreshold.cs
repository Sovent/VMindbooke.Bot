namespace Usage.Domain.ValueObjects.LikeThresholds
{
    public class UserLikesThreshold
    {
        public UserLikesThreshold(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}