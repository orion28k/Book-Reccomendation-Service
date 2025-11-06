using System.Runtime.InteropServices.Marshalling;
using System.Xml.Serialization;

namespace BookRec.Domain.BookModel;

/// <summary>
/// Represents a book model entity with unique Ids, title, author, description, and publish date.
/// </summary>
/// 
public class Book : Entity
{
    /// <value>Entity Identifier</value>
    public Guid Id { get; private set; } = Guid.NewGuid;

    /// Model Properties
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Description { get; private set; }
    public DateTime PublishDate { get; private set; }

    /// <summary> 
    /// Private Constructor for Entity Framework and deserialization tools
    /// </summary>
    private Book() { }

    /// <summary> 
    /// Public Book Constructor 
    /// </summary>
    public Book(string title, string author, string description, DateTime publishDate)
    {
        setTitle(title);
        setAuthor(author);
        setDescription(description);
        PublishDate = publishDate;
    }

    /// <summary>
    /// The Methods below add business logic to safely set the model's properties. 
    /// Method's are set to private because once they are created they should 
    /// not be updated.
    /// </summary>
    private void setTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title can not be empty.");
        }
        Title = title;
    }

    private void setAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ArgumentException("Author can not be empty.");
        }
        Author = author;
    }

    private void setDescription(string description)
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