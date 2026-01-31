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

    [MapperIgnoreSource(nameof(GalleryStorageContract.PartitionKey))]
    public partial Gallery StorageContractToDomainModel(GalleryStorageContract storageContract);

    [MapperIgnoreTarget(nameof(GalleryStorageContract.PartitionKey))]
    private partial GalleryStorageContract MapToStorageContract(Gallery gallery);
}