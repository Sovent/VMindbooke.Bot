using Bogus;
using Usage.Domain.Entities;

namespace Usage.Domain
{
    public class CommentContentProvider : ICommentContentProvider
    {
        private readonly Faker _faker = new Faker("ru");

        public CommentContent GetComment()
        {
            return new CommentContent(_faker.Lorem.Sentence());
        }
    }
}