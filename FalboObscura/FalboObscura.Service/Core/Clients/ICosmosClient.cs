// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Microsoft.Azure.Cosmos;

namespace FalboObscura.Core.Clients;

public interface ICosmosClient
{
    /// <summary>
    /// Gets a database reference for the specified database name
    /// </summary>
    Database GetDatabase(string databaseName);
    
    /// <summary>
    /// Gets a container reference for the specified database and container names
    /// </summary>
    Container GetContainer(string databaseName, string containerName);
    
    /// <summary>
    /// Creates a new item in the specified container
    /// </summary>
    Task<ItemResponse<T>> CreateItemAsync<T>(T item, string databaseName, string containerName, PartitionKey? partitionKey = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reads an item from the specified container
    /// </summary>
    Task<ItemResponse<T>> ReadItemAsync<T>(string id, string databaseName, string containerName, PartitionKey partitionKey, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an item in the specified container
    /// </summary>
    Task<ItemResponse<T>> ReplaceItemAsync<T>(T item, string id, string databaseName, string containerName, PartitionKey? partitionKey = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an item from the specified container
    /// </summary>
    Task<ItemResponse<T>> DeleteItemAsync<T>(string id, string databaseName, string containerName, PartitionKey partitionKey, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a query against the specified container
    /// </summary>
    FeedIterator<T> GetItemQueryIterator<T>(QueryDefinition queryDefinition, string databaseName, string containerName, string? continuationToken = null, QueryRequestOptions? requestOptions = null);
    
    /// <summary>
    /// Executes a query against the specified container with SQL string
    /// </summary>
    FeedIterator<T> GetItemQueryIterator<T>(string queryText, string databaseName, string containerName, string? continuationToken = null, QueryRequestOptions? requestOptions = null);
}