// ------------------------------------
// Falbo Obscura
// ------------------------------------

namespace FalboObscura.Core.Models;

public class Gallery
{
    public string Description { get; set; } = "Gallery of images.";

    public List<GalleryImage> GalleryImages { get; set; } = default!;

    public string GalleryName { get; set; } = "Gallery";

    public string GalleryType { get; set; } = default!;

    public Guid Id { get; set; } = Guid.NewGuid();

    public int Order { get; set; }
}