namespace BookRec.Application.Books.Dtos;

/// <summary>
/// This record creates a Book Input DTO that will return information to the backend from the presentation layer.
/// </summary>
public sealed record CreateBookDto(
    string Title,
    string Author,
    string Description,
    IReadOnlyCollection<string> Genre,
    DateTime PublishDate
);