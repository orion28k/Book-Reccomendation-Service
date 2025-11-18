namespace BookRec.Application.Books.Dtos;

public sealed record BookDto(
    Guid Id,
    string title,
    string author,
    string description,
    string genre,
    DateTime publishDate
);