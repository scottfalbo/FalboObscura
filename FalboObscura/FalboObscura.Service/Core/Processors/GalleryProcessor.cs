// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Repositories;

namespace FalboObscura.Core.Processors;

public class GalleryProcessor(
    IGalleryRepository galleryRepository,
    IBlobStorageProcessor blobStorageProcessor) : IGalleryProcessor
{
    private readonly IBlobStorageProcessor _blobStorageProcessor = blobStorageProcessor ?? throw new ArgumentNullException(nameof(blobStorageProcessor));
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

    public async Task<bool> DeleteGallery(Gallery gallery)
    {
        try
        {
            await _galleryRepository.DeleteGallery(gallery.Id, gallery.GalleryType);
            return true;
        }
        catch (Exception)
        {
            // TODO: implement exception handling
            return false;
        }
    }

    public async Task<IEnumerable<Gallery>> GetGalleries(string galleryType)
    {
        try
        {
            var galleries = await _galleryRepository.GetGalleries(galleryType);
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

    public async Task<bool> AddGalleryImage(ImageUpload imageUpload, Gallery gallery)
    {
        try
        {
            var galleryImage = new GalleryImage()
            {
                AltText = imageUpload.AltText,
                Description = imageUpload.Description,
                Title = imageUpload.Title,
            };

            imageUpload.Id = galleryImage.Id;

            var blobUrl = await _blobStorageProcessor.StoreImage(imageUpload);

            galleryImage.ImageUrl = blobUrl;
            galleryImage.ImageThumbnailUrl = $"{blobUrl}-thumbnail";

            gallery.GalleryImages.Add(galleryImage);
            await _galleryRepository.UpdateGallery(gallery);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // TODO: implement exception handling
            return false;
        }
    }

    public async Task<bool> RemoveGalleryImage(GalleryImage galleryImage, Gallery gallery)
    {
        try
        {
            gallery.GalleryImages.Remove(galleryImage);
            await _galleryRepository.UpdateGallery(gallery);

            return true;
        }
        catch (Exception)
        {
            // TODO: implement exception handling
            return false;
        }
    }
}