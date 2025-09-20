// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Repositories;

namespace FalboObscura.Core.Processors;

public class GalleryProcessor(
    IGalleryImageRepository galleryImageRepository,
    IBlobStorageProcessor blobStorageProcessor) : IGalleryProcessor
{
    private readonly IBlobStorageProcessor _blobStorageProcessor = blobStorageProcessor ?? throw new ArgumentNullException(nameof(blobStorageProcessor));
    private readonly IGalleryImageRepository _galleryImageRepository = galleryImageRepository ?? throw new ArgumentNullException(nameof(galleryImageRepository));

    public async Task CreateGalleryImage(GalleryImage galleryImage)
    {
        try
        {
            await _galleryImageRepository.CreateGalleryImage(galleryImage);
        }
        catch (Exception)
        {
            // TODO: implement exception handling
        }
    }

    public async Task<bool> DeleteGalleryImage(Guid id, string imageType)
    {
        try
        {
            await _galleryImageRepository.DeleteGalleryImage(id, imageType);
            return true;
        }
        catch (Exception)
        {
            // TODO: implement exception handling
            return false;
        }
    }

    public async Task<IEnumerable<GalleryImage>> GetGalleryImages(string imageType)
    {
        try
        {
            var galleryImages = await _galleryImageRepository.GetGalleryImages(imageType);

            return galleryImages;
        }
        catch (Exception)
        {
            // TODO: implement exception handling
            return [];
        }
    }

    public async Task UpdateGalleryImage(GalleryImage galleryImage)
    {
        try
        {
            await _galleryImageRepository.UpdateGalleryImage(galleryImage);
        }
        catch (Exception)
        {
            // TODO: implement exception handling
        }
    }
}