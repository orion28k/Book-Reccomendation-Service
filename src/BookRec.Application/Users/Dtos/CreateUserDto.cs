namespace BookRec.Application.Users.Dtos;

/// <summary>
/// This record creates a User Input DTO that will return information to the backend from the presentation layer.
/// </summary>
public sealed record CreateUserDto(
    string Username,
    string FirstName,
    string LastName,
    string Email,
    IEnumerable<string> PreferredGenres
);