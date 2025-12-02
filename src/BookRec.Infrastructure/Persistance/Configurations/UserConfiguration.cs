using BookRec.Infrastructure.Dbos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookRec.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserDBO>
{
    public void Configure(EntityTypeBuilder<UserDBO> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Username).IsRequired().HasMaxLength(100);
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PreferredGenres).IsRequired();
        builder.Property(x => x.createdAt).IsRequired();
        builder.Property(x => x.updatedAt).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Username).IsUnique();
    }
}