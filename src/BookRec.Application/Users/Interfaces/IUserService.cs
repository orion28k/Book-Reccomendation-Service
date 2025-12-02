using BookRec.Application.Users.Dtos;

namespace BookRec.Application.Users.Interface;

// TODO: Add Update User
public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByUserAsync(string username);
    Task<IReadOnlyList<string>> GetUserPreferredGenresAsync(Guid id);
    Task<Guid> AddUser(CreateUserDto user);
    Task UpdatePreferredGenresAsync(Guid userId, IEnumerable<string> preferredGenres);
    Task DeleteUser(Guid id);
}