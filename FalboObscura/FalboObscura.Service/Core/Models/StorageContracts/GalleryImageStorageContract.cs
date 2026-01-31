// ------------------------------------
// Falbo Obscura
// ------------------------------------

using System.Text.Json.Serialization;

namespace FalboObscura.Core.Models.StorageContracts;

public class GalleryImageStorageContract
{
    public string AltText { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid GalleryId { get; set; } = Guid.NewGuid();

    [JsonRequired]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ImageThumbnailUrl { get; set; } = string.Empty;

    public string ImageType { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; set; } = string.Empty;

    public string? Title { get; set; }
}