using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
namespace WebApplication1.Models;

public class Books
{
    [Key]
    public string Isbn { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string? Author { get; set; }

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Publisher { get; set; } = string.Empty;

    [Required]
    public string ThumbnailUrl { get; set; } = string.Empty;

    [Required]
    public DateTime? PublishedDate { get; set; }

    [Required]
    public int PageCount { get; set; }

    [Required]
    public float AverageRating { get; set; }
}