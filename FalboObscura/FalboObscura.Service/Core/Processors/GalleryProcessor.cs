// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Repositories;

namespace FalboObscura.Core.Processors;

public class GalleryProcessor(
    IGalleryRepository galleryRepository,
    IGalleryImageRepository galleryImageRepository,
    IBlobStorageProcessor blobStorageProcessor) : IGalleryProcessor
{
    private readonly IBlobStorageProcessor _blobStorageProcessor = blobStorageProcessor ?? throw new ArgumentNullException(nameof(blobStorageProcessor));
    private readonly IGalleryImageRepository _galleryImageRepository = galleryImageRepository ?? throw new ArgumentNullException(nameof(galleryImageRepository));
    private readonly IGalleryRepository _galleryRepository = galleryRepository ?? throw new ArgumentNullException(nameof(galleryRepository));

    public async Task<Gallery> CreateGallery(Gallery gallery)
    {
        try
        {
            await _galleryRepository.CreateGallery(gallery);
            return gallery;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // TODO: implement exception handling
            throw new Exception("Failed to create gallery", ex);
        }
    }

    public async Task<bool> DeleteGallery(Guid id)
    {
        try
        {
            await _galleryRepository.DeleteGallery(id, "Gallery");
            return true;
        }
        catch (Exception)
        {
            // TODO: implement exception handling
            return false;
        }
    }

    public async Task<IEnumerable<Gallery>> GetGalleries()
    {
        try
        {
            var galleries = await _galleryRepository.GetGalleries("Gallery");
            return galleries;
        }
        catch (Exception)
        {
            // TODO: implement exception handling
            return [];
        }
    }

    public async Task UpdateGallery(Gallery gallery)
    {
        try
        {
            await _galleryRepository.UpdateGallery(gallery);
        }
        catch (Exception)
        {
            // TODO: implement exception handling
        }
    }

    public async Task<GalleryImage> CreateGalleryImage(ImageUpload imageUpload)
    {
        try
        {
            var galleryImage = new GalleryImage()
            {
                AltText = imageUpload.AltText,
                Description = imageUpload.Description,
                ImageType = imageUpload.ImageType,
                Title = imageUpload.Title,
            };

            imageUpload.Id = galleryImage.Id;

            var blobUrl = await _blobStorageProcessor.StoreImage(imageUpload);

            galleryImage.ImageUrl = blobUrl;
            galleryImage.ImageThumbnailUrl = $"{blobUrl}-thumbnail";

            await _galleryImageRepository.CreateGalleryImage(galleryImage);

            return galleryImage;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // TODO: implement exception handling
            throw new Exception("womp womp", ex);
        }
    }

    public async Task<bool> DeleteGalleryImage(Guid id, string imageType)
    {
        try
        {
            await _galleryImageRepository.DeleteGalleryImage(id, imageType);
            await _blobStorageProcessor.DeleteImage(id, imageType);
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