namespace LikesCheating.Domain
{
    public class Thresholds
    {
        public int PostThresholdToComment { get; }
        public int PostThresholdToDuplicate { get; }
        public int CommentThreshold { get; }
        public int UserThreshold { get; }

        public Thresholds(int postThresholdToComment, int postThresholdToDuplicate,
            int commentThreshold, int userThreshold)
        {
            PostThresholdToComment = postThresholdToComment;
            PostThresholdToDuplicate = postThresholdToDuplicate;
            CommentThreshold = commentThreshold;
            UserThreshold = userThreshold;
        }
    }
}