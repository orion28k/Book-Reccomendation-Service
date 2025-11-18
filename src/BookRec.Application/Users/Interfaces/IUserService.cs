using BookRec.Application.Users.Dtos;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByUserAsync(string username);
    Task<IReadOnlyList<UserDto>> GetByPreferredGenre(string genre);
    Task<Guid> AddUser(CreateUserDto user);
    Task UpdatePreferredGenresAsync(Guid userId, IEnumerable<string> preferredGenres);
    Task DeleteUser(UserDto user);
}