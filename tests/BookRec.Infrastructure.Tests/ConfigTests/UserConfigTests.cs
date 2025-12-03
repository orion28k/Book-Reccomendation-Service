using BookRec.Infrastructure.Dbos;
using BookRec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookRec.Infrastructure.Tests.Persistence;

public class UserConfigurationTests : IDisposable
{
    private readonly AppDbContext _context;

    public UserConfigurationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public void UserDBO_ShouldHavePrimaryKey()
    {
        var entityType = _context.Model.FindEntityType(typeof(UserDBO));
        var primaryKey = entityType?.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        Assert.Equal("Id", primaryKey.Properties[0].Name);
    }

    [Fact]
    public void UserDBO_Username_ShouldBeRequired()
    {
        var entityType = _context.Model.FindEntityType(typeof(UserDBO));
        var property = entityType?.FindProperty("Username");

        Assert.NotNull(property);
        Assert.False(property.IsNullable);
    }

    [Fact]
    public void UserDBO_Username_ShouldHaveMaxLength100()
    {
        var entityType = _context.Model.FindEntityType(typeof(UserDBO));
        var property = entityType?.FindProperty("Username");

        Assert.Equal(100, property?.GetMaxLength());
    }

    [Fact]
    public void UserDBO_Email_ShouldHaveMaxLength100()
    {
        var entityType = _context.Model.FindEntityType(typeof(UserDBO));
        var property = entityType?.FindProperty("Email");

        Assert.Equal(100, property?.GetMaxLength());
    }

    [Fact]
    public void UserDBO_FirstName_ShouldHaveMaxLength100()
    {
        var entityType = _context.Model.FindEntityType(typeof(UserDBO));
        var property = entityType?.FindProperty("FirstName");

        Assert.Equal(100, property?.GetMaxLength());
    }

    [Fact]
    public void UserDBO_LastName_ShouldHaveMaxLength100()
    {
        var entityType = _context.Model.FindEntityType(typeof(UserDBO));
        var property = entityType?.FindProperty("LastName");

        Assert.Equal(100, property?.GetMaxLength());
    }

    [Fact]
    public void UserDBO_ShouldHaveUniqueIndexOnEmail()
    {
        var entityType = _context.Model.FindEntityType(typeof(UserDBO));
        var indexes = entityType?.GetIndexes().ToList();
        var emailIndex = indexes?.FirstOrDefault(i => i.Properties.Any(p => p.Name == "Email"));

        Assert.NotNull(emailIndex);
        Assert.True(emailIndex.IsUnique);
    }

    [Fact]
    public void UserDBO_ShouldHaveUniqueIndexOnUsername()
    {
        var entityType = _context.Model.FindEntityType(typeof(UserDBO));
        var indexes = entityType?.GetIndexes().ToList();
        var usernameIndex = indexes?.FirstOrDefault(i => i.Properties.Any(p => p.Name == "Username"));

        Assert.NotNull(usernameIndex);
        Assert.True(usernameIndex.IsUnique);
    }
}