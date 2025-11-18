namespace BookRec.Application.Users.Dtos;

public sealed record UserDto(
    Guid Id,
    string username,
    string firstName,
    string lastName,
    string email,
    IReadOnlyCollection<string> preferredGenres,
    DateTime createdAt
);