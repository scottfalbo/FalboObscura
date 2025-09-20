// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace FalboObscura.Core.Clients;

public class BlobStorageClient(BlobServiceClient blobServiceClient) : IBlobStorageClient
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));

    public async Task<string> UploadBlobAsync(string containerName, string blobName, Stream content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var options = new BlobUploadOptions();
        if (!string.IsNullOrEmpty(contentType))
        {
            options.HttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        }

        await blobClient.UploadAsync(content, options, cancellationToken);
        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteBlobAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }
}