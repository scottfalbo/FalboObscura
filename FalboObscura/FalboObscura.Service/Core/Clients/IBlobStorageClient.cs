// ------------------------------------
// Falbo Obscura
// ------------------------------------

namespace FalboObscura.Core.Clients;

public interface IBlobStorageClient
{
    Task<bool> DeleteBlobAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

    Task<string> UploadBlobAsync(string containerName, string blobName, Stream content, string? contentType = null, CancellationToken cancellationToken = default);
}