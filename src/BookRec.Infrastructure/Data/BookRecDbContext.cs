using Microsoft.EntityFrameworkCore;

namespace BookRec.Infrastructure.Data;

// TODO: Replace these with actual domain namespaces/classes if different
public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<string> PreferredGenres { get; set; } = new();
}

public class BookRecDbContext : DbContext
{
    public BookRecDbContext(DbContextOptions<BookRecDbContext> options) : base(options) {}

    public DbSet<Book> Books => Set<Book>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(b =>
        {
            b.ToTable("books");
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Author).IsRequired().HasMaxLength(200);
            b.Property(x => x.Genre).IsRequired().HasMaxLength(100);
            b.Property(x => x.Description).IsRequired().HasMaxLength(800);
            b.Property(x => x.PublishDate).IsRequired();
            b.HasIndex(x => x.Title);
            b.HasIndex(x => x.Author);
        });

        modelBuilder.Entity<User>(u =>
        {
            u.ToTable("users");
            u.HasKey(x => x.Id);
            u.Property(x => x.Username).IsRequired().HasMaxLength(100);
            u.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            u.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            u.Property(x => x.Email).IsRequired().HasMaxLength(200);
            u.Property(x => x.CreatedAt).IsRequired();
            u.Property(x => x.PreferredGenres)
             .HasConversion(
                v => string.Join(",", v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        });
    }
}
