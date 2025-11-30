using Microsoft.EntityFrameworkCore;
using BookRec.Infrastructure.Data;

namespace BookRec.Infrastructure.Repositories;

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

public class EfUserRepository : IUserRepository
{
    private readonly BookRecDbContext _db;
    public EfUserRepository(BookRecDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id)!;

    public Task<User?> GetByEmailAsync(string email) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email)!;

    public Task<User?> GetByUserAsync(string username) =>
        _db.Users.FirstOrDefaultAsync(u => u.Username == username)!;

    public async Task<IReadOnlyList<User>> GetByPreferredGenre(string genre) =>
        await _db.Users.Where(u => u.PreferredGenres.Contains(genre)).ToListAsync();

    public async Task AddUser(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteUser(User user)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateUser(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}
