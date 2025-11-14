using BookRec.Domain.BookModel;

namespace BookRec.Domain.UserModel;

/// <summary>
/// This Repository Interface defines the CRUD operations that will be implemented in the Infrastructure layer.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserAsync(string username);
    Task<IReadOnlyList<User>> GetByPreferredGenre(string genre);
    Task AddUser(User user);
    Task DeleteUser(User user);
    Task UpdateUser(User user);
}