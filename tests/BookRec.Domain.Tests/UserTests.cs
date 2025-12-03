using BookRec.Domain.UserModel;

namespace BookRec.Domain.Tests;

public class UserTests
{
    private User CreateValidUser() => new(
        Guid.NewGuid(),
        "testuser",
        "John",
        "Doe",
        "john@test.com",
        new List<string> { "Fiction" },
        DateTime.UtcNow
    );

    [Fact]
    public void Constructor_ValidData_CreatesUser()
    {
        var user = CreateValidUser();

        Assert.Equal("testuser", user.Username);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("john@test.com", user.Email);
        Assert.Single(user.PreferredGenres);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetUsername_InvalidUsername_Throws(string? username)
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.setUserName(username!));
    }

    [Theory]
    [InlineData("test")]      // Too short (4 chars)
    [InlineData("abcdefghijklmnopqrstuvwxyz12345")]  // Too long (31 chars)
    public void SetUsername_InvalidLength_Throws(string username)
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.setUserName(username));
    }

    [Fact]
    public void SetUsername_ValidUsername_Updates()
    {
        var user = CreateValidUser();

        user.setUserName("newuser123");

        Assert.Equal("newuser123", user.Username);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetFirstName_InvalidFirstName_Throws(string? firstName)
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.setFirstName(firstName!));
    }

    [Fact]
    public void SetFirstName_ValidFirstName_Updates()
    {
        var user = CreateValidUser();

        user.setFirstName("Jane");

        Assert.Equal("Jane", user.FirstName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetLastName_InvalidLastName_Throws(string? lastName)
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.setLastName(lastName!));
    }

    [Fact]
    public void SetLastName_ValidLastName_Updates()
    {
        var user = CreateValidUser();

        user.setLastName("Smith");

        Assert.Equal("Smith", user.LastName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateEmail_InvalidEmail_Throws(string? email)
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.updateEmail(email!));
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public void UpdateEmail_InvalidFormat_Throws(string email)
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.updateEmail(email));
    }

    [Fact]
    public void UpdateEmail_ValidEmail_Updates()
    {
        var user = CreateValidUser();

        user.updateEmail("new@email.com");

        Assert.Equal("new@email.com", user.Email);
    }

    [Fact]
    public void AddPreferredGenre_ValidGenre_AddsToList()
    {
        var user = CreateValidUser();

        user.addPreferredGenre("Mystery");

        Assert.Equal(2, user.PreferredGenres.Count);
        Assert.Contains("Mystery", user.PreferredGenres);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddPreferredGenre_InvalidGenre_Throws(string? genre)
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.addPreferredGenre(genre!));
    }

    [Fact]
    public void AddPreferredGenre_DuplicateGenre_Throws()
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.addPreferredGenre("Fiction"));
    }

    [Fact]
    public void AddPreferredGenre_DuplicateDifferentCase_Throws()
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.addPreferredGenre("FICTION"));
    }

    [Fact]
    public void UpdatePreferredGenres_ValidGenres_ReplacesAll()
    {
        var user = CreateValidUser();

        user.updatePreferredGenres(new List<string> { "Horror", "Thriller" });

        Assert.Equal(2, user.PreferredGenres.Count);
        Assert.DoesNotContain("Fiction", user.PreferredGenres);
    }

    [Fact]
    public void UpdatePreferredGenres_NullOrEmpty_Throws()
    {
        var user = CreateValidUser();

        Assert.Throws<ArgumentException>(() => user.updatePreferredGenres(null!));
        Assert.Throws<ArgumentException>(() => user.updatePreferredGenres(new List<string>()));
    }
}