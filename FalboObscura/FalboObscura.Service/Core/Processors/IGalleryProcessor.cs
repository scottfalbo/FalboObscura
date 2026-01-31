// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Processors;

public interface IGalleryProcessor
{
    public Task<Gallery> CreateGallery(Gallery gallery);

    public Task<bool> DeleteGallery(Guid id);

    public Task<Gallery?> GetGallery(Guid id);

    public Task UpdateGallery(Gallery gallery);

    public Task<GalleryImage> CreateGalleryImage(ImageUpload imageUpload);

    public Task<bool> DeleteGalleryImage(Guid id, string imageType);

    public Task<GalleryImage?> GetGalleryImage(Guid id, string imageType);

    public Task UpdateGalleryImage(GalleryImage galleryImage);
}