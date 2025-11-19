namespace BookRec.Application.Books.Dtos;

/// <summary>
/// This record creates a Book Output DTO that will return information to the API to
/// prevent data leaks
/// </summary>
public sealed record BookDto(
    Guid Id,
    string Title,
    string Author,
    string Description,
    string Genre,
    DateTime PublishDate
);