// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models.Constants;

namespace FalboObscura.Core.Models;

public class GalleryImage
{
    public string AltText { get; set; } = default!;

    public string? Description { get; set; }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string ImageThumbnailUrl { get; set; } = default!;

    public ImageType ImageType { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;

    public string PartitionKey { get; set; } = default!;

    public string? Title { get; set; }
}