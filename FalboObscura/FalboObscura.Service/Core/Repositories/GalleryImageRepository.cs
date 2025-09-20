// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Clients;
using FalboObscura.Core.Models;
using FalboObscura.Core.Models.StorageContracts;
using Microsoft.Azure.Cosmos;

namespace FalboObscura.Core.Repositories;

public class GalleryImageRepository(ICosmosClient cosmosClient) : IGalleryImageRepository
{
    private readonly string _containerName = "Images";
    private readonly ICosmosClient _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
    private readonly string _databaseName = "FalboObscura";
    private readonly GalleryImageMapper _mapper = new();

    public async Task CreateGalleryImage(GalleryImage galleryImage)
    {
        var storageContract = _mapper.DomainModelToStorageContract(galleryImage);
        var partitionKey = new PartitionKey(storageContract.PartitionKey);

        await _cosmosClient.UpsertItemAsync(
            storageContract,
            _databaseName,
            _containerName,
            partitionKey);
    }

    public async Task DeleteGalleryImage(Guid id, string partitionKey)
    {
        try
        {
            var partitionKeyValue = new PartitionKey(partitionKey);

            await _cosmosClient.DeleteItemAsync<GalleryImageStorageContract>(
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

    public async Task<IEnumerable<GalleryImage>> GetGalleryImages(string partitionKey)
    {
        var partitionKeyValue = new PartitionKey(partitionKey);

        var storageContracts = await _cosmosClient.GetItemsByPartitionKeyAsync<GalleryImageStorageContract>(
            _databaseName,
            _containerName,
            partitionKeyValue);

        var galleryImages = storageContracts.Select(_mapper.StorageContractToDomainModel);

        return galleryImages;
    }

    public async Task<GalleryImage> UpdateGalleryImage(GalleryImage galleryImage)
    {
        var storageContract = _mapper.DomainModelToStorageContract(galleryImage);
        var partitionKey = new PartitionKey(storageContract.PartitionKey);

        await _cosmosClient.UpsertItemAsync(
            storageContract,
            _databaseName,
            _containerName,
            partitionKey);

        return galleryImage;
    }
}