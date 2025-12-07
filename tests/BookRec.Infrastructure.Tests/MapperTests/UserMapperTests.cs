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
        var dbo = new UserDBO
        {
            Id = id,
            Username = "johndoe123",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PreferredGenres = "Fiction,Mystery",
            createdAt = DateTime.UtcNow
        };

        var result = UserMapper.ToDomain(dbo);

        Assert.Equal(id, result.Id);
        Assert.Equal("johndoe123", result.Username);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal(2, result.PreferredGenres.Count);
        Assert.Contains("Fiction", result.PreferredGenres);
        Assert.Contains("Mystery", result.PreferredGenres);
    }

    [Fact]
    public void ToDomain_ShouldHandleSingleGenre()
    {
        var dbo = new UserDBO 
        { 
            Id = Guid.NewGuid(),
            Username = "testuser1",
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PreferredGenres = "Fiction" 
        };

        var result = UserMapper.ToDomain(dbo);

        Assert.Single(result.PreferredGenres);
        Assert.Contains("Fiction", result.PreferredGenres);
    }

    [Fact]
    public void ToDBO_ShouldMapAllProperties()
    {
        var id = Guid.NewGuid();
        var user = new User(id, "johndoe123", "John", "Doe", "john@example.com", 
            new List<string> { "Fiction", "Mystery" }, DateTime.UtcNow);

        var result = UserMapper.ToDBO(user);

        Assert.Equal(id, result.Id);
        Assert.Equal("johndoe123", result.Username);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal("Fiction,Mystery", result.PreferredGenres);
    }

    [Fact]
    public void ToDBO_ShouldSetTimestamps()
    {
        var before = DateTime.UtcNow;
        var user = new User(Guid.NewGuid(), "testuser1", "First", "Last", "email@test.com", 
            new List<string> { "Fiction" }, DateTime.UtcNow);

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
            new UserDBO { Id = Guid.NewGuid(), Username = "userone11", FirstName = "User", LastName = "One", Email = "one@test.com", PreferredGenres = "Fiction" },
            new UserDBO { Id = Guid.NewGuid(), Username = "usertwo22", FirstName = "User", LastName = "Two", Email = "two@test.com", PreferredGenres = "Mystery" }
        };

        var result = UserMapper.ToDomainList(dbos);

        Assert.Equal(2, result.Count);
        Assert.Equal("userone11", result[0].Username);
        Assert.Equal("usertwo22", result[1].Username);
    }

    [Fact]
    public void ToDomainList_ShouldReturnEmptyList_WhenEmpty()
    {
        var dbos = new List<UserDBO>();

        var result = UserMapper.ToDomainList(dbos);

        Assert.Empty(result);
    }
}