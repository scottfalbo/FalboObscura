// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Microsoft.Azure.Cosmos;

namespace FalboObscura.Core.Clients;

public interface ICosmosClient
{
    Container GetContainer(string databaseName, string containerName);

    Database GetDatabase(string databaseName);

    Task<IEnumerable<T>> GetItemsByPartitionKeyAsync<T>(string databaseName, string containerName, PartitionKey partitionKey, CancellationToken cancellationToken = default);

    Task<ItemResponse<T>> UpsertItemAsync<T>(T item, string databaseName, string containerName, PartitionKey? partitionKey = null, CancellationToken cancellationToken = default);

    Task<ItemResponse<T>> DeleteItemAsync<T>(string id, string databaseName, string containerName, PartitionKey partitionKey, CancellationToken cancellationToken = default);
}