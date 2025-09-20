// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace FalboObscura.Core.Clients;

public class BlobStorageClient(BlobServiceClient blobServiceClient) : IBlobStorageClient
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));

    public async Task<bool> BlobExistsAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = GetContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.ExistsAsync(cancellationToken);
        return response.Value;
    }

    public async Task<bool> DeleteBlobAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = GetContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    public async Task<Stream> DownloadBlobAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = GetContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return response.Value.Content;
    }

    public async Task<byte[]> DownloadBlobToByteArrayAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = GetContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadContentAsync(cancellationToken);
        return response.Value.Content.ToArray();
    }

    public string GetBlobUrl(string containerName, string blobName)
    {
        var containerClient = GetContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        return blobClient.Uri.ToString();
    }

    public BlobContainerClient GetContainerClient(string containerName)
    {
        return _blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async IAsyncEnumerable<BlobItem> ListBlobsAsync(string containerName, string? prefix = null, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var containerClient = GetContainerClient(containerName);

        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            yield return blobItem;
        }
    }

    public async Task<BlobClient> UploadBlobAsync(string containerName, string blobName, Stream content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var containerClient = GetContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var options = new BlobUploadOptions();
        if (!string.IsNullOrEmpty(contentType))
        {
            options.HttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        }

        await blobClient.UploadAsync(content, options, cancellationToken);
        return blobClient;
    }

    public async Task<BlobClient> UploadBlobAsync(string containerName, string blobName, byte[] content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(content);
        return await UploadBlobAsync(containerName, blobName, stream, contentType, cancellationToken);
    }
}