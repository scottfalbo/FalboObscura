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
    [MapProperty(nameof(GalleryImage.ImageType), nameof(GalleryImageStorageContract.PartitionKey))]
    public partial GalleryImageStorageContract DomainModelToStorageContract(GalleryImage galleryImage);

    [MapperIgnoreSource(nameof(GalleryImageStorageContract.PartitionKey))] // Ignore PartitionKey when mapping back
    public partial GalleryImage StorageContractToDomainModel(GalleryImageStorageContract storageContract);
}