// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Repositories;

namespace FalboObscura.Core.Processors;

public class GalleryProcessor(
    IGalleryImageRepository imageRepository,
    IBlobStorageProcessor blobStorageProcessor) : IGalleryProcessor
{
    private readonly IBlobStorageProcessor _blobStorageProcessor = blobStorageProcessor ?? throw new ArgumentNullException(nameof(blobStorageProcessor));
    private readonly IGalleryImageRepository _galleryImageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));

    public Task CreateGalleryImage(GalleryImage galleryImage)
    {
        throw new NotImplementedException();
    }

    public Task DeleteGalleryImage(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<GalleryImage>> GetGalleryImages(string imageType)
    {
        var galleryImages = await _galleryImageRepository.GetGalleryImages(imageType);

        return galleryImages;
    }

    public Task UpdateGalleryImage(GalleryImage galleryImage)
    {
        throw new NotImplementedException();
    }
}