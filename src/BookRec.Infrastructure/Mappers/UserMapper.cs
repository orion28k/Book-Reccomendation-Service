
using System.Data.Common;
using BookRec.Domain.BookModel;
using BookRec.Domain.UserModel;
using BookRec.Infrastructure.Dbos;

namespace BookRec.Infrastructure.Mappers;

public static class UserMapper
{
    public static User ToDomain(UserDBO userDBO)
    {
        var user = new User(
            userDBO.Id,
            userDBO.Username,
            userDBO.FirstName,
            userDBO.LastName,
            userDBO.Email,
            string.IsNullOrEmpty(userDBO.PreferredGenres) ? new List<string>() : userDBO.PreferredGenres.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            userDBO.createdAt
        );

        if (!string.IsNullOrEmpty(userDBO.ReadBookIds))
        {
            var ids = userDBO.ReadBookIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();

            user.updateReadBooks(ids);
        }

        return user;
    }

    public static UserDBO ToDBO(User user)
    {
        return new UserDBO
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PreferredGenres = string.Join(",", user.PreferredGenres),
            ReadBookIds = user.ReadBooks == null ? string.Empty : string.Join(",", user.ReadBooks),
            createdAt = DateTime.UtcNow,
            updatedAt = DateTime.UtcNow
        };
    }

    public static List<User> ToDomainList(IEnumerable<UserDBO> dbos)
    {
        return dbos.Select(ToDomain).ToList();
    }
}