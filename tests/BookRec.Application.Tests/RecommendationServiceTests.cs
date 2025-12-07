using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using BookRec.Application.Books.Services;
using BookRec.Application.Users.Interface;
using BookRec.Domain.BookModel;
using BookRec.Application.Books.Dtos;
using BookRec.Domain.UserModel;

namespace BookRec.Application.Tests
{
    public class RecommendationServiceTests
    {
        [Fact]
        public async Task RecommendBooksForUserAsync_ExcludesReadBooks_AndMatchesGenres()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var readBookId = Guid.NewGuid();

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetUserPreferredGenresAsync(userId))
                .ReturnsAsync(new List<string> { "Fantasy", "Sci-Fi" });
            mockUserService.Setup(s => s.GetUserReadBookIdsAsync(userId))
                .ReturnsAsync(new List<Guid> { readBookId });

            var book1 = new Book(Guid.NewGuid(), "A", "Author", "Desc", new[] { "Fantasy" }, DateTime.UtcNow.AddYears(-1));
            var book2 = new Book(readBookId, "B", "Author2", "Desc2", new[] { "Sci-Fi" }, DateTime.UtcNow.AddYears(-2));
            var book3 = new Book(Guid.NewGuid(), "C", "Author3", "Desc3", new[] { "Mystery" }, DateTime.UtcNow);

            var mockBookRepo = new Mock<BookRec.Domain.BookModel.IBookRepository>();
            mockBookRepo.Setup(r => r.GetByGenreAsync("Fantasy")).ReturnsAsync(new List<Book> { book1 });
            mockBookRepo.Setup(r => r.GetByGenreAsync("Sci-Fi")).ReturnsAsync(new List<Book> { book2 });

            var service = new RecommendationService(mockUserService.Object, mockBookRepo.Object);

            // Act
            var results = await service.RecommendBooksForUserAsync(userId, 10);

            // Assert
            Assert.NotNull(results);
            var ids = results.Select(r => r.Id).ToList();
            Assert.Contains(book1.Id, ids);
            Assert.DoesNotContain(book2.Id, ids); // read book excluded
            Assert.DoesNotContain(book3.Id, ids); // different genre
        }
    }
}
