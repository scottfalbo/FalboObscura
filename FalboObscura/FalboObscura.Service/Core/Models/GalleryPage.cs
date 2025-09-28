// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models.Constants;

namespace FalboObscura.Core.Models;

public class GalleryPage
{
    public string Description { get; set; } = "This is a webpage.";
    public Guid Id { get; set; } = Guid.NewGuid();

    public string PageType { get; set; } = MetadataType.GalleryPage;
    public string Title { get; set; } = "Page";
}