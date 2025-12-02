using BookRec.Application.Books.Dtos;
using BookRec.Application.Users.Dtos;
using BookRec.Infrastructure.Dbos;
using Microsoft.EntityFrameworkCore;

namespace BookRec.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BookDBO> Books => Set<BookDBO>();
    public DbSet<UserDBO> Users => Set<UserDBO>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}