// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Clients;

namespace FalboObscura.Core.Processors;

public class BlobStorageProcessor(IBlobStorageClient client) : IBlobStorageProcessor
{
    private readonly IBlobStorageClient _client = client;
}