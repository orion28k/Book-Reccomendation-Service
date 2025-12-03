using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using BookRec.Application.Users.Services;
using BookRec.Domain.UserModel;

namespace BookRec.Application.Tests
{
    public class UserServiceReadTests
    {
        [Fact]
        public async Task MarkBookAsReadAsync_AddsBookAndCallsUpdate()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            var user = new User(userId, "tester", "First", "Last", "a@b.com", new[] { "Fantasy" }, DateTime.UtcNow);

            var mockRepo = new Mock<BookRec.Domain.UserModel.IUserRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask).Verifiable();

            var service = new UserService(mockRepo.Object);

            // Act
            await service.MarkBookAsReadAsync(userId, bookId);

            // Assert
            mockRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.ReadBookIds.Contains(bookId))), Times.Once);
        }

        [Fact]
        public async Task UnmarkBookAsReadAsync_RemovesBookAndCallsUpdate()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            var user = new User(userId, "tester2", "First", "Last", "b@b.com", new[] { "Sci-Fi" }, DateTime.UtcNow);
            user.UpdateReadBooks(new[] { bookId });

            var mockRepo = new Mock<BookRec.Domain.UserModel.IUserRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask).Verifiable();

            var service = new UserService(mockRepo.Object);

            // Act
            await service.UnmarkBookAsReadAsync(userId, bookId);

            // Assert
            mockRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => !u.ReadBookIds.Contains(bookId))), Times.Once);
        }

        [Fact]
        public async Task GetUserReadBookIdsAsync_ReturnsReadIds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var book1 = Guid.NewGuid();
            var book2 = Guid.NewGuid();

            var user = new User(userId, "tester3", "First", "Last", "c@b.com", new[] { "Mystery" }, DateTime.UtcNow);
            user.UpdateReadBooks(new[] { book1, book2 });

            var mockRepo = new Mock<BookRec.Domain.UserModel.IUserRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object);

            // Act
            var result = await service.GetUserReadBookIdsAsync(userId);

            // Assert
            Assert.Contains(book1, result);
            Assert.Contains(book2, result);
            Assert.Equal(2, result.Count);
        }
    }
}
