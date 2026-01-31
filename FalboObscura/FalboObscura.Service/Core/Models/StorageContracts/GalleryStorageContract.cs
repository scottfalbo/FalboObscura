// ------------------------------------
// Falbo Obscura
// ------------------------------------

using System.Text.Json.Serialization;

namespace FalboObscura.Core.Models.StorageContracts;

public class GalleryStorageContract
{
    public string Description { get; set; } = string.Empty;

    public IEnumerable<GalleryImage> GalleryImages { get; set; } = default!;

    public string GalleryName { get; set; } = string.Empty;

    public string GalleryType { get; set; } = string.Empty;

    [JsonRequired]
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Order { get; set; }

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; } = "Gallery";
}