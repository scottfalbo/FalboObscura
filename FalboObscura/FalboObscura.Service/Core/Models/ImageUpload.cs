// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Microsoft.AspNetCore.Components.Forms;

namespace FalboObscura.Core.Models;

public class ImageUpload
{
    public string AltText { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid Id { get; set; }

    public IBrowserFile? ImageFile { get; set; }

    public string ImageType { get; set; } = default!;

    public string? Title { get; set; }
}