using System.Runtime.InteropServices.Marshalling;
using System.Xml.Serialization;

namespace BookRec.Domain.BookModel;

/// <summary>
/// Represents a book model entity with unique Ids, title, author, description, and publish date.
/// </summary>
/// 
public class Book : Entity
{
    /// Model Properties
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    private List<string> _genres = new();
    public List<string> Genres
    {
        get => _genres;
        set => _genres = value ?? new();
    }
    public DateTime PublishDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary> 
    /// Private Constructor for Entity Framework and deserialization tools
    /// </summary>
    private Book(Guid id) : base(id) { }

    /// <summary> 
    /// Public Book Constructor 
    /// </summary>
    public Book(Guid id, string title, string author, string description, IEnumerable<string> genre, DateTime publishDate) : base(id)
    {
        setTitle(title);
        setAuthor(author);
        setDescription(description);
        setGenre(genre);
        PublishDate = publishDate;
    }

    /// <summary>
    /// The Methods below add business logic to safely set the model's properties. 
    /// Method's are set to private because once they are created they should 
    /// not be updated.
    /// </summary>
    public void setTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title can not be empty.");
        }
        Title = title;
    }

    public void setAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ArgumentException("Author can not be empty.");
        }
        Author = author;
    }

    public void setGenre(IEnumerable<string> Genre)
    {
        if (Genre == null || !Genre.Any())
        {
            throw new ArgumentException("Users must have at least 1 Genre.");
        }

        foreach (var genre in Genre)
        {
            addGenre(genre);
        }
    }
    
    public void addGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
        {
            throw new ArgumentException("Genre can not be empty.");
        }

        genre = genre.Trim();

        if (_genres.Contains(genre, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Can not add Genre: " + genre + ". It already exists.");
        }

        _genres.Add(genre);
    }

    public void updateGenres(IEnumerable<string> genres)
    {
        if(genres == null || !genres.Any())
        {
            throw new ArgumentException("Users must have at least 1 Genre.");
        }

        _genres.Clear();

        foreach (var genre in genres)
        {
            addGenre(genre);
        }
    }

    public void setDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description can not be empty.");
        }
        if (description.Length > 800)
        {
            throw new ArgumentException("Description can not be longer than 800 characters.");
        }
        Description = description;
    } 
}