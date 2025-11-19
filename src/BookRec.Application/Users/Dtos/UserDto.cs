namespace BookRec.Application.Users.Dtos;

/// <summary>
/// This record creates a User Output DTO that will return information to presentation layer from the backend to prevent data leaks.
/// </summary>
public sealed record UserDto(
    Guid Id,
    string Username,
    string FirstName,
    string LastName,
    string Email,
    IReadOnlyCollection<string> PreferredGenres,
    DateTime CreatedAt
);