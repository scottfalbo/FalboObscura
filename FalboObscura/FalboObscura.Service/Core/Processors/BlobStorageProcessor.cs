// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Clients;
using FalboObscura.Core.Models;

namespace FalboObscura.Core.Processors;

public class BlobStorageProcessor(IBlobStorageClient client) : IBlobStorageProcessor
{
    private const string ContainerName = "falbo-obscura";
    private readonly IBlobStorageClient _client = client;

    public async Task<string> StoreImage(ImageUpload imageUpload)
    {
        var image = imageUpload.ImageFile ?? throw new ArgumentException("ImageFile cannot be null", nameof(imageUpload));

        var filename = $"{imageUpload.ImageType}-{imageUpload.Id}";

        const long maxFileSize = 10 * 1024 * 1024; // 10 MB
        using var stream = image.OpenReadStream(maxFileSize);
        var blobUrl = await _client.UploadBlobAsync(ContainerName, filename, stream, image.ContentType);

        return blobUrl;
    }
}