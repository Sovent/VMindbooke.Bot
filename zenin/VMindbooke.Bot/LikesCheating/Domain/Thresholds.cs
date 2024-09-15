namespace LikesCheating.Domain
{
    public class Thresholds
    {
        public int PostThresholdToComment { get; }
        public int PostThresholdToDuplicate { get; }
        public int CommentThresholdToReply { get; }
        public int UserSuccessPostThreshold { get; }
        public int UserDailyTargetThreshold { get; }

        public Thresholds(int postThresholdToComment, int postThresholdToDuplicate,
            int commentThresholdToReply, int userSuccessPostThreshold, int userDailyTargetThreshold)
        {
            PostThresholdToComment = postThresholdToComment;
            PostThresholdToDuplicate = postThresholdToDuplicate;
            CommentThresholdToReply = commentThresholdToReply;
            UserSuccessPostThreshold = userSuccessPostThreshold;
            UserDailyTargetThreshold = userDailyTargetThreshold;
        }
    }
}