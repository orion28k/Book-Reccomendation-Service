using BookRec.Application.Users.Dtos;

namespace BookRec.Application.Users.Interface;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByUserAsync(string username);
    Task<IReadOnlyList<string>> GetUserPreferredGenresAsync(Guid id);
    Task<IReadOnlyList<Guid>> GetUserReadBookIdsAsync(Guid id);
    Task MarkBookAsReadAsync(Guid userId, Guid bookId);
    Task<Guid> AddUser(CreateUserDto user);
    Task<Guid> UpdateUser(UpdateUserDto user, Guid id);
    Task DeleteUser(Guid id);
}