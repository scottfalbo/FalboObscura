// ------------------------------------
// Falbo Obscura
// ------------------------------------

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace FalboObscura.Core.Models;

public class ImageUpload
{
    [Required(ErrorMessage = "Alt text is required for accessibility")]
    [StringLength(200, ErrorMessage = "Alt text cannot exceed 200 characters")]
    public string AltText { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Please select an image file")]
    public IBrowserFile? ImageFile { get; set; }

    public string? Title { get; set; }
}