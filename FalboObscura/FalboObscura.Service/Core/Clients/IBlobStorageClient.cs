// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace FalboObscura.Core.Clients;

public interface IBlobStorageClient
{
    /// <summary>
    /// Gets a blob container client for the specified container name
    /// </summary>
    BlobContainerClient GetContainerClient(string containerName);
    
    /// <summary>
    /// Uploads a blob to the specified container
    /// </summary>
    Task<BlobClient> UploadBlobAsync(string containerName, string blobName, Stream content, string? contentType = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Uploads a blob to the specified container from byte array
    /// </summary>
    Task<BlobClient> UploadBlobAsync(string containerName, string blobName, byte[] content, string? contentType = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Downloads a blob from the specified container
    /// </summary>
    Task<Stream> DownloadBlobAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Downloads blob content to a byte array
    /// </summary>
    Task<byte[]> DownloadBlobToByteArrayAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a blob from the specified container
    /// </summary>
    Task<bool> DeleteBlobAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a blob exists in the specified container
    /// </summary>
    Task<bool> BlobExistsAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists blobs in the specified container with optional prefix
    /// </summary>
    IAsyncEnumerable<BlobItem> ListBlobsAsync(string containerName, string? prefix = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the URL for a blob
    /// </summary>
    string GetBlobUrl(string containerName, string blobName);
}