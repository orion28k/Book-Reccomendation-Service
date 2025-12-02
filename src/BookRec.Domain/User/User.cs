using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Net.Mail;
using Microsoft.VisualBasic;

namespace BookRec.Domain.UserModel;

/// <summary>description</summary>
public class User : Entity
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    private readonly List<string> _preferredGenres = new();
    public IReadOnlyCollection<string> PreferredGenres => _preferredGenres.AsReadOnly();
    public DateTime createdAt { get; set; } = DateTime.UtcNow;


    public User(Guid id, string username, string firstName, string lastName, string email, IEnumerable<string> preferredGenre, DateTime createdAt) : base(id)
    {
        setUserName(username);
        setFirstName(firstName);
        setLastName(lastName);
        updateEmail(email);
        setPreferredGenre(preferredGenre);
    }

    public void setPreferredGenre(IEnumerable<string> preferredGenre)
    {
        if (preferredGenre == null || !preferredGenre.Any())
        {
            throw new ArgumentException("Users must have at least 1 Genre.");
        }

        foreach (var genre in preferredGenre)
        {
            addPreferredGenre(genre);
        }
    }
    
    public void addPreferredGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
        {
            throw new ArgumentException("Genre can not be empty.");
        }

        genre = genre.Trim();

        if (_preferredGenres.Contains(genre, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Can not add Genre: " + genre + ". It already exists.");
        }

        _preferredGenres.Add(genre);
    }

    public void updatePreferredGenres(IEnumerable<string> preferredGenres)
    {
        if(preferredGenres == null || !preferredGenres.Any())
        {
            throw new ArgumentException("Users must have at least 1 Genre.");
        }

        _preferredGenres.Clear();

        foreach (var genre in preferredGenres)
        {
            addPreferredGenre(genre);
        }
    }

    public void setUserName(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username can not be empty.");
        }
        if (username.Length > 30 || username.Length < 5)
        {
            throw new ArgumentException("Username must have at least 5 characters and no more than 30 characters.");
        }
        /// <summary>Restricts the use of certain characters within a username</summary>
        if (!System.Text.RegularExpressions.Regex.IsMatch(username, "[A-Za-z0-9_.']"))
        {
            throw new ArgumentException("Username has invalid characters use only [A-Za-z0-9_.'].");
        }
        Username = username;
    }
    public void setFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("FirstName can not be empty.");
        }
        /// <summary>Restricts the use of certain characters within a username</summary>
        if (!System.Text.RegularExpressions.Regex.IsMatch(firstName, "[A-Za-z_']"))
        {
            throw new ArgumentException("FirstName has invalid characters use only [A-Za-z_']].");
        }
        FirstName = firstName;
    }
    public void setLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("LastName can not be empty.");
        }
        /// <summary>Restricts the use of certain characters within a username</summary>
        if (!System.Text.RegularExpressions.Regex.IsMatch(lastName, "[A-Za-z_']"))
        {
            throw new ArgumentException("LastName has invalid characters use only [A-Za-z_'].");
        }
        LastName = lastName;
    }
    public void updateEmail(string email)
    {
        setEmail(email);
    }
    private void setEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email can not be empty.");
        }

        email = email.Trim();

        try
        {
            /// <summary>
            /// Uses .Net Mail class to automatically check if the email 
            /// has the correct email format
            /// </summary>
            var formattedEmail = new MailAddress(email);
            email = formattedEmail.Address;
        }
        catch
        {
            throw new ArgumentException("Invalid Email Format");
        }

        Email = email;
    }

}