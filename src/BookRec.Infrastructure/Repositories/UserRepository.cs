using BookRec.Infrastructure.Mappers;
using BookRec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BookRec.Domain.UserModel;

namespace BookRec.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var dbo = await _db.Users.FirstOrDefaultAsync(b => b.Id == id);
        return dbo is null ? null : UserMapper.ToDomain(dbo);
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        var dbo = await _db.Users.FirstOrDefaultAsync(b => b.Email == email);
        return dbo is null ? null : UserMapper.ToDomain(dbo);
    }
    public async Task<User?> GetByUserAsync(string username)
    {
        var dbo = await _db.Users.FirstOrDefaultAsync(b => b.Username == username);
        return dbo is null ? null : UserMapper.ToDomain(dbo);
    }
    public async Task<List<User>> GetAllAsync()
    {
        var dbos = await _db.Users.OrderBy(b => b.Id).ToListAsync();
        return UserMapper.ToDomainList(dbos);
    }
    public async Task AddAsync(User user)
    {
        var dbo = UserMapper.ToDBO(user);
        dbo.createdAt = DateTime.UtcNow;
        _db.Users.Add(dbo);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        var dbUser = await _db.Users.FirstOrDefaultAsync(b => b.Id == user.Id);
        if (dbUser is null) return;

        dbUser.Username = user.Username;
        dbUser.FirstName = user.FirstName;
        dbUser.LastName = user.LastName;
        dbUser.Email = user.Email;
        dbUser.PreferredGenres = user.PreferredGenres == null ? "" : string.Join(",", user.PreferredGenres);
        dbUser.ReadBookIds = user.ReadBooks == null ? string.Empty : string.Join(",", user.ReadBooks);
        dbUser.updatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var dbo = await _db.Users.FirstOrDefaultAsync(b => b.Id == id);
        if (dbo is null) return;

        _db.Users.Remove(dbo);
        await _db.SaveChangesAsync();
    }

}