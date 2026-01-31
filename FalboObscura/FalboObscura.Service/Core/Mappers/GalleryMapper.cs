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

        return galleryStorageContract;
    }

    [MapperIgnoreSource(nameof(GalleryStorageContract.PartitionKey))]
    public partial Gallery StorageContractToDomainModel(GalleryStorageContract storageContract);

    private partial GalleryStorageContract MapToStorageContract(Gallery gallery);
}
