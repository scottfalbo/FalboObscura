// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Microsoft.Azure.Cosmos;

namespace FalboObscura.Core.Clients;

public class CosmosClient(Microsoft.Azure.Cosmos.CosmosClient cosmosClient) : ICosmosClient, IDisposable
{
    private readonly Microsoft.Azure.Cosmos.CosmosClient _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
    private bool _disposed = false;

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

    public async Task<IEnumerable<T>> GetItemsByPartitionKeyAsync<T>(string databaseName, string containerName, PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var container = GetContainer(databaseName, containerName);
        var queryDefinition = new QueryDefinition("SELECT * FROM c");
        var requestOptions = new QueryRequestOptions { PartitionKey = partitionKey };
        
        var iterator = container.GetItemQueryIterator<T>(queryDefinition, requestOptions: requestOptions);
        var results = new List<T>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }

    public async Task<ItemResponse<T>> UpsertItemAsync<T>(T item, string databaseName, string containerName, PartitionKey? partitionKey = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var container = GetContainer(databaseName, containerName);
        return await container.UpsertItemAsync(item, partitionKey, cancellationToken: cancellationToken);
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