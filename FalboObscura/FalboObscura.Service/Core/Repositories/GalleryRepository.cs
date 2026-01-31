// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Clients;
using FalboObscura.Core.Models;
using FalboObscura.Core.Models.StorageContracts;
using Microsoft.Azure.Cosmos;

namespace FalboObscura.Core.Repositories;

public class GalleryRepository(ICosmosClient cosmosClient) : IGalleryRepository
{
    private readonly string _containerName = "Gallery";
    private readonly ICosmosClient _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
    private readonly string _databaseName = "FalboObscura";
    private readonly GalleryMapper _mapper = new();

    public async Task CreateGallery(Gallery gallery)
    {
        var storageContract = _mapper.DomainModelToStorageContract(gallery);
        var partitionKey = new PartitionKey(storageContract.PartitionKey);

        await _cosmosClient.UpsertItemAsync(
            storageContract,
            _databaseName,
            _containerName,
            partitionKey);
    }

    public async Task DeleteGallery(Guid id, string partitionKey)
    {
        try
        {
            var partitionKeyValue = new PartitionKey(partitionKey);

            await _cosmosClient.DeleteItemAsync<GalleryStorageContract>(
                id.ToString(),
                _databaseName,
                _containerName,
                partitionKeyValue);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // do nothing
        }
    }

    public async Task<Gallery?> GetGallery(Guid id, string partitionKey)
    {
        try
        {
            var partitionKeyValue = new PartitionKey(partitionKey);

            var storageContract = await _cosmosClient.GetItemAsync<GalleryStorageContract>(
                id.ToString(),
                _databaseName,
                _containerName,
                partitionKeyValue);

            return _mapper.StorageContractToDomainModel(storageContract);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Gallery> UpdateGallery(Gallery gallery)
    {
        var storageContract = _mapper.DomainModelToStorageContract(gallery);
        var partitionKey = new PartitionKey(storageContract.PartitionKey);

        await _cosmosClient.UpsertItemAsync(
            storageContract,
            _databaseName,
            _containerName,
            partitionKey);

        return gallery;
    }
}
