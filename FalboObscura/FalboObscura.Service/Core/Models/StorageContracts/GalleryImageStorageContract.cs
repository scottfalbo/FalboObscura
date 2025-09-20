// ------------------------------------
// Falbo Obscura
// ------------------------------------

using System.Text.Json.Serialization;

namespace FalboObscura.Core.Models.StorageContracts;

public class GalleryImageStorageContract
{
    public string AltText { get; set; } = default!;

    public string? Description { get; set; }

    [JsonRequired]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ImageThumbnailUrl { get; set; } = default!;

    public string ImageType { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;

    public string PartitionKey { get; set; } = default!;

    public string? Title { get; set; }
}