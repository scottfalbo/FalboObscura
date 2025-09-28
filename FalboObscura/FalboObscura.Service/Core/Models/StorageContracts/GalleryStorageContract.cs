// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models.Constants;
using System.Text.Json.Serialization;

namespace FalboObscura.Core.Models.StorageContracts;

public class GalleryStorageContract
{
    public string GalleryDescription { get; set; } = "Gallery of images.";

    public IEnumerable<GalleryImage> GalleryImages { get; set; } = default!;

    public string GalleryName { get; set; } = "Gallery";

    [JsonRequired]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; } = MetadataType.Gallery;
}