using BookRec.Infrastructure.Dbos;
using Xunit;

namespace BookRec.Infrastructure.Tests.Dbos;

public class UserDBOTests
{
    [Fact]
    public void UserDBO_ShouldInitializeWithDefaultValues()
    {
        var user = new UserDBO();

        Assert.Equal(Guid.Empty, user.Id);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PreferredGenres);
    }

    [Fact]
    public void UserDBO_ShouldSetAndGetProperties()
    {
        var id = Guid.NewGuid();

        var user = new UserDBO
        {
            Id = id,
            Username = "johndoe",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PreferredGenres = "Fiction,Mystery"
        };

        Assert.Equal(id, user.Id);
        Assert.Equal("johndoe", user.Username);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("Fiction,Mystery", user.PreferredGenres);
    }

    [Fact]
    public void UserDBO_createdAt_ShouldDefaultToUtcNow()
    {
        var before = DateTime.UtcNow;
        var user = new UserDBO();
        var after = DateTime.UtcNow;

        Assert.True(user.createdAt >= before);
        Assert.True(user.createdAt <= after);
    }

    [Fact]
    public void UserDBO_updatedAt_ShouldDefaultToUtcNow()
    {
        var before = DateTime.UtcNow;
        var user = new UserDBO();
        var after = DateTime.UtcNow;

        Assert.True(user.updatedAt >= before);
        Assert.True(user.updatedAt <= after);
    }
}