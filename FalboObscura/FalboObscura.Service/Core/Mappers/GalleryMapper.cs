// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Models.StorageContracts;
using Riok.Mapperly.Abstractions;

namespace FalboObscura.Core;

[Mapper]
public partial class GalleryMapper
{
    public GalleryStorageContract DomainModelToStorageContract(Gallery gallery)
    {
        var galleryStorageContract = MapToStorageContract(gallery);
        galleryStorageContract.PartitionKey = gallery.GalleryType;

        return galleryStorageContract;
    }

    public Gallery StorageContractToDomainModel(GalleryStorageContract storageContract)
    {
        var gallery = MapToDomainModel(storageContract);
        // Ensure GalleryType is set from PartitionKey if not already populated
        if (string.IsNullOrEmpty(gallery.GalleryType))
        {
            gallery.GalleryType = storageContract.PartitionKey;
        }
        return gallery;
    }

    [MapperIgnoreSource(nameof(GalleryStorageContract.PartitionKey))]
    private partial Gallery MapToDomainModel(GalleryStorageContract storageContract);

    [MapperIgnoreTarget(nameof(GalleryStorageContract.PartitionKey))]
    private partial GalleryStorageContract MapToStorageContract(Gallery gallery);
}