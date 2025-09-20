// ------------------------------------
// Falbo Obscura
// ------------------------------------

namespace FalboObscura.Core.Models;

public class GalleryImage
{
    public string AltText { get; set; } = default!;

    public string? Description { get; set; }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string ImageThumbnailUrl { get; set; } = default!;

    public string ImageType { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;

    public string? Title { get; set; }
}