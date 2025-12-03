using BookRec.Application.Books.Dtos;
using BookRec.Application.Books.Interface;
using BookRec.Application.Users.Interface;
using BookRec.Domain.BookModel;

namespace BookRec.Application.Books.Services;

/// <summary>
/// This Book Service defines the implemented functions from the Book Service Interface
/// </summary>
public class RecommendationService : IRecommendationService
{
    private readonly IUserService _userService;
    private readonly IBookRepository _bookRepository;

    public RecommendationService(IUserService userService, IBookRepository bookRepository)
    {
        _userService = userService;
        _bookRepository = bookRepository;
    }

    public async Task<IReadOnlyList<BookDto>> RecommendBooksForUserAsync(Guid userId, int limit = 20)
    {
        // Get preferred genres
        var preferredGenres = await _userService.GetUserPreferredGenresAsync(userId);
        if (preferredGenres == null || !preferredGenres.Any()) return new List<BookDto>();

        // Gather candidate books for each preferred genre
        var candidates = new List<Book>();
        foreach (var genre in preferredGenres)
        {
            var books = await _bookRepository.GetByGenreAsync(genre);
            if (books != null) candidates.AddRange(books);
        }

        // Get read book ids for user to exclude
        var readIds = await _userService.GetUserReadBookIdsAsync(userId);
        var readSet = new HashSet<Guid>(readIds ?? Array.Empty<Guid>());

        // Filter out read books, dedupe, order by publish date desc and take limit
        var filtered = candidates
            .Where(b => !readSet.Contains(b.Id))
            .GroupBy(b => b.Id)
            .Select(g => g.First())
            .OrderByDescending(b => b.PublishDate)
            .Take(limit)
            .ToList();

        return filtered.Select(MapToDto).ToList();
    }

    private static BookDto MapToDto(Book book) => new BookDto(
        book.Id,
        book.Title,
        book.Author,
        book.Description,
        book.Genres,
        book.PublishDate
    );
}