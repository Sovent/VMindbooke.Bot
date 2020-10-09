using NUnit.Framework;
using VMindBooke.SDK.Application;

namespace CarmaFucker.Tests
{
    public class CarmaFuckerTests
    {
        [Test]
        public void CommentLikedPosts_ValidData_NoException()
        {
            // Arrange
            var client = new VMindBookeClient(new UserServiceMock(), new PostServiceMock());
            var carmaFucker = new CarmaFucker(client);

            // Act
            carmaFucker.CommentLikedPosts();

            // Assert
            Assert.Pass();
        }

        [Test]
        public void IsContinueCheating_ValidData_NoException()
        {
            // Arrange
            var client = new VMindBookeClient(new UserServiceMock(), new PostServiceMock());
            var carmaFucker = new CarmaFucker(client);

            // Act
            carmaFucker.IsContinueCheating();

            // Assert
            Assert.Pass();
        }

        [Test]
        public void ReplyLikedComments_ValidData_NoException()
        {
            // Arrange
            var client = new VMindBookeClient(new UserServiceMock(), new PostServiceMock());
            var carmaFucker = new CarmaFucker(client);

            // Act
            carmaFucker.ReplyLikedComments();

            // Assert
            Assert.Pass();
        }

        [Test]
        public void CopyPostLikedPosts_ValidData_NoException()
        {
            // Arrange
            var client = new VMindBookeClient(new UserServiceMock(), new PostServiceMock());
            var carmaFucker = new CarmaFucker(client);

            // Act
            carmaFucker.CopyPostLikedPosts();

            // Assert
            Assert.Pass();
        }

        [Test]
        public void CopyPostLikedUserPost_ValidData_NoException()
        {
            // Arrange
            var client = new VMindBookeClient(new UserServiceMock(), new PostServiceMock());
            var carmaFucker = new CarmaFucker(client);

            // Act
            carmaFucker.CopyPostLikedUserPost();

            // Assert
            Assert.Pass();
        }
    }
}