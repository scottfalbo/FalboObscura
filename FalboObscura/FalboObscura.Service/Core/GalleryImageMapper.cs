// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Models.StorageContracts;
using Riok.Mapperly.Abstractions;

namespace FalboObscura.Core;

[Mapper]
public partial class GalleryImageMapper
{
    public GalleryImageStorageContract DomainModelToStorageContract(GalleryImage galleryImage)
    {
        var galleryImageStorageContract = MapToStorageContract(galleryImage);
        galleryImageStorageContract.PartitionKey = galleryImage.ImageType;

        return galleryImageStorageContract;
    }

    [MapperIgnoreSource(nameof(GalleryImageStorageContract.PartitionKey))]
    public partial GalleryImage StorageContractToDomainModel(GalleryImageStorageContract storageContract);

    [MapProperty(nameof(GalleryImage.ImageType), nameof(GalleryImageStorageContract.PartitionKey))]
    private partial GalleryImageStorageContract MapToStorageContract(GalleryImage galleryImage);
}