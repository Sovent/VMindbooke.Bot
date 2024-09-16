using System;
using NUnit.Framework;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.UnitTests
{
    public class PostTests
    {
        [Test]
        public void IsValid_ValidPostObjectInInput()
        {
            //arrange
            var post = new Post(1, 1, "title", "content", DateTime.Now, new Comment[0], new Like[0]);
            
            //act
            var isValid = post.IsValid();

            //assert
            Assert.IsTrue(isValid);
        }
    }
}