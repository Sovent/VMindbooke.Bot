using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.UnitTests
{
    public class ValidCollectionExtensionTests
    {
        [Test]
        public void GetValidCollection_ReturnCollectionOnlyWithValidObjects()
        {
            //arrange
            var users = new List<User>()
            {
                new User(1, "token1", "name1", new Like[0]),
                new User(2, "token2", null, new Like[0]),
                new User(3, "token3", "name3", new Like[0])
            };
            
            //act
            var validUsers = users.GetValidCollection<User>();

            //assert
            Assert.AreEqual(2, validUsers.Count());
        }
    }
}