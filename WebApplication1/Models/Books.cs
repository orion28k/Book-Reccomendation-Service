using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
namespace WebApplication1.Models;

public class Books
{
    [Key]
    public string Isbn { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Author { get; set; }

    public string? Genre { get; set; }

    public string? Description { get; set; }

    public string? Publisher { get; set; }

    public string? ThumbnailUrl { get; set; }

    public DateTime? PublishedDate { get; set; }

    public int? PageCount { get; set; }

    public float? AverageRating { get; set; }
}