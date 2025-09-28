// ------------------------------------
// Falbo Obscura
// ------------------------------------

namespace FalboObscura.Core.Models;

public class Gallery
{
    public string GalleryDescription { get; set; } = "Gallery of images.";

    public IEnumerable<GalleryImage> GalleryImages { get; set; } = default!;

    public string GalleryName { get; set; } = "Gallery";

    public Guid Id { get; set; } = Guid.NewGuid();
}