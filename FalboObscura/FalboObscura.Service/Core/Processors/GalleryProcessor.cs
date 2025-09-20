// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Models.Constants;
using FalboObscura.Core.Repositories;

namespace FalboObscura.Core.Processors;

public class GalleryProcessor(
    IGalleryImageRepository imageRepository,
    IBlobStorageProcessor blobStorageProcessor) : IGalleryProcessor
{
    private readonly IBlobStorageProcessor _blobStorageProcessor = blobStorageProcessor ?? throw new ArgumentNullException(nameof(blobStorageProcessor));
    private readonly IGalleryImageRepository _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));

    public void CreateGalleryImage()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<GalleryImage> GetGalleryImages(ImageType imageType)
    {
        throw new NotImplementedException();
    }
}