using BookRec.Infrastructure.Dbos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookRec.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<BookDBO>
{
    public void Configure(EntityTypeBuilder<BookDBO> builder)
    {
        builder.ToTable("books");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Author).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(800);
        builder.Property(x => x.Genre).IsRequired();
        builder.Property(x => x.PublishDate).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => x.Title);
        builder.HasIndex(x => x.Author);
        builder.HasIndex(x => x.Genre);
    }
}