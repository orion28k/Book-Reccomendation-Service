namespace BookRec.Application.Books.Dtos;

/// <summary>
/// This record creates a Book Update DTO that will return information to the backend from the presentation layer.
/// </summary>
public sealed record UpdateBookDto(
    string Title,
    string Author,
    string Description,
    IEnumerable<string> Genre,
    DateTime PublishDate,
    DateTime UpdatedAt
);