using System;
using System.Collections.Generic;
using Bogus;
using Bogus.DataSets;
using VMindBooke.SDK.Domain;

namespace CarmaFucker.Tests
{
    public static class FakeDataGenerator
    {
        public static Post GetPost(int id = -1, int userId = -1)
        {
            var faker = new Faker("ru");
            if (id == -1) id = faker.Random.Int(0, 500);
            if (userId == -1) userId = faker.Random.Int(0, 500);
            var post = new Post(
                id,
                userId,
                faker.Lorem.Sentence(5, 2),
                faker.Lorem.Sentence(10, 5),
                faker.Date.Recent(2, DateTime.Now),
                new List<Comment>()
                {
                    GetComment(),
                    GetComment(),
                    GetComment(),
                    GetComment()
                },
                new List<Like>()
                {
                    GetLike(),
                    GetLike(),
                    GetLike(),
                    GetLike(),
                    GetLike(),
                    GetLike(),
                }
            );
            return post;
        }

        public static Like GetLike()
        {
            var faker = new Faker("ru");
            var like = new Like(
                Guid.NewGuid(),
                faker.Random.Int(0, 500),
                faker.Date.Recent(3, DateTime.Now)
            );
            return like;
        }

        public static Comment GetComment()
        {
            var faker = new Faker("ru");
            var comment = new Comment(
                Guid.NewGuid(),
                faker.Random.Int(0, 500),
                faker.Lorem.Sentence(5, 1),
                faker.Date.Recent(3, DateTime.Now),
                new List<Comment>(),
                new List<Like>()
            );
            return comment;
        }

        public static User GetUser(int id = -1)
        {
            var faker = new Faker("ru");
            if (id == -1) id = faker.Random.Int(0, 500);
            var user = new User(
                id,
                faker.Random.String(8, 10),
                faker.Name.FullName(Name.Gender.Male),
                new List<Like>()
                {
                    GetLike(),
                    GetLike(),
                    GetLike(),
                    GetLike(),
                }
            );
            return user;
        }
    }
}