// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Microsoft.Azure.Cosmos;

namespace FalboObscura.Core.Clients;

public class CosmosClient(Microsoft.Azure.Cosmos.CosmosClient cosmosClient) : ICosmosClient, IDisposable
{
    private readonly Microsoft.Azure.Cosmos.CosmosClient _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
    private bool _disposed = false;

    public async Task<ItemResponse<T>> CreateItemAsync<T>(T item, string databaseName, string containerName, PartitionKey? partitionKey = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var container = GetContainer(databaseName, containerName);
        return await container.CreateItemAsync(item, partitionKey, cancellationToken: cancellationToken);
    }

    public async Task<ItemResponse<T>> DeleteItemAsync<T>(string id, string databaseName, string containerName, PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var container = GetContainer(databaseName, containerName);
        return await container.DeleteItemAsync<T>(id, partitionKey, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Container GetContainer(string databaseName, string containerName)
    {
        ThrowIfDisposed();
        return _cosmosClient.GetContainer(databaseName, containerName);
    }

    public Database GetDatabase(string databaseName)
    {
        ThrowIfDisposed();
        return _cosmosClient.GetDatabase(databaseName);
    }

    public FeedIterator<T> GetItemQueryIterator<T>(QueryDefinition queryDefinition, string databaseName, string containerName, string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        ThrowIfDisposed();
        var container = GetContainer(databaseName, containerName);
        return container.GetItemQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public FeedIterator<T> GetItemQueryIterator<T>(string queryText, string databaseName, string containerName, string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        ThrowIfDisposed();
        var container = GetContainer(databaseName, containerName);
        return container.GetItemQueryIterator<T>(queryText, continuationToken, requestOptions);
    }

    public async Task<ItemResponse<T>> ReadItemAsync<T>(string id, string databaseName, string containerName, PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var container = GetContainer(databaseName, containerName);
        return await container.ReadItemAsync<T>(id, partitionKey, cancellationToken: cancellationToken);
    }

    public async Task<ItemResponse<T>> ReplaceItemAsync<T>(T item, string id, string databaseName, string containerName, PartitionKey? partitionKey = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var container = GetContainer(databaseName, containerName);
        return await container.ReplaceItemAsync(item, id, partitionKey, cancellationToken: cancellationToken);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _cosmosClient?.Dispose();
            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(CosmosClient));
    }
}