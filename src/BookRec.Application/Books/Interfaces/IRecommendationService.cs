using BookRec.Application.Books.Dtos;

namespace BookRec.Application.Books.Interface;

public interface IRecommendationService
{
    Task<IReadOnlyList<BookDto>> RecommendBooksForUserAsync(Guid userId, int limit = 10);
}