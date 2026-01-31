// ------------------------------------
// Falbo Obscura
// ------------------------------------

using System.ComponentModel.DataAnnotations;

namespace FalboObscura.Core.Models;

public class GalleryImage
{
    [Required(ErrorMessage = "Alt text is required")]
    public string AltText { get; set; } = default!;

    public string? Description { get; set; }

    public Guid GalleryId { get; set; }

    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Image thumbnail URL is required")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string ImageThumbnailUrl { get; set; } = default!;

    public string ImageType { get; set; } = default!;

    [Required(ErrorMessage = "Image URL is required")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string ImageUrl { get; set; } = default!;

    public string? Title { get; set; }
}