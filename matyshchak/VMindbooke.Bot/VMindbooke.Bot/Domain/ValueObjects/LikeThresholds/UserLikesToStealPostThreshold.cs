namespace Usage.Domain.ValueObjects.LikeThresholds
{
    public class UserLikesToStealPostThreshold
    {
        public UserLikesToStealPostThreshold(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}