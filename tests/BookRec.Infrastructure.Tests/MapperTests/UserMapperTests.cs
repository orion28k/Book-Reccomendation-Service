using BookRec.Domain.UserModel;
using BookRec.Infrastructure.Dbos;
using BookRec.Infrastructure.Mappers;
using Xunit;

namespace BookRec.Infrastructure.Tests.Mappers;

public class UserMapperTests
{
    [Fact]
    public void ToDomain_ShouldMapAllProperties()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2023, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        var dbo = new UserDBO
        {
            Id = id,
            Username = "johndoe",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PreferredGenres = "Fiction,Mystery",
            createdAt = createdAt
        };

        var result = UserMapper.ToDomain(dbo);

        Assert.Equal(id, result.Id);
        Assert.Equal("johndoe", result.Username);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal(new List<string> { "Fiction", "Mystery" }, result.PreferredGenres);
        Assert.Equal(createdAt, result.CreatedAt);
    }

    [Fact]
    public void ToDomain_ShouldReturnEmptyList_WhenPreferredGenresIsEmpty()
    {
        var dbo = new UserDBO { PreferredGenres = string.Empty };

        var result = UserMapper.ToDomain(dbo);

        Assert.Empty(result.PreferredGenres);
    }

    [Fact]
    public void ToDomain_ShouldRemoveEmptyEntries_WhenGenresHaveEmptyValues()
    {
        var dbo = new UserDBO { PreferredGenres = "Fiction,,Mystery," };

        var result = UserMapper.ToDomain(dbo);

        Assert.Equal(2, result.PreferredGenres.Count);
        Assert.Contains("Fiction", result.PreferredGenres);
        Assert.Contains("Mystery", result.PreferredGenres);
    }

    [Fact]
    public void ToDBO_ShouldMapAllProperties()
    {
        var id = Guid.NewGuid();
        var user = new User(id, "johndoe", "John", "Doe", "john@example.com", 
            new List<string> { "Fiction", "Mystery" }, DateTime.UtcNow);

        var result = UserMapper.ToDBO(user);

        Assert.Equal(id, result.Id);
        Assert.Equal("johndoe", result.Username);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal("Fiction,Mystery", result.PreferredGenres);
    }

    [Fact]
    public void ToDBO_ShouldSetTimestamps()
    {
        var before = DateTime.UtcNow;
        var user = new User(Guid.NewGuid(), "user", "First", "Last", "email@test.com", 
            new List<string>(), DateTime.UtcNow);

        var result = UserMapper.ToDBO(user);
        var after = DateTime.UtcNow;

        Assert.True(result.createdAt >= before && result.createdAt <= after);
        Assert.True(result.updatedAt >= before && result.updatedAt <= after);
    }

    [Fact]
    public void ToDomainList_ShouldMapAllItems()
    {
        var dbos = new List<UserDBO>
        {
            new UserDBO { Id = Guid.NewGuid(), Username = "user1" },
            new UserDBO { Id = Guid.NewGuid(), Username = "user2" }
        };

        var result = UserMapper.ToDomainList(dbos);

        Assert.Equal(2, result.Count);
        Assert.Equal("user1", result[0].Username);
        Assert.Equal("user2", result[1].Username);
    }

    [Fact]
    public void ToDomainList_ShouldReturnEmptyList_WhenEmpty()
    {
        var dbos = new List<UserDBO>();

        var result = UserMapper.ToDomainList(dbos);

        Assert.Empty(result);
    }
}