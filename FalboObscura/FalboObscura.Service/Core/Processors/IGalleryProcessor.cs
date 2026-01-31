// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Processors;

public interface IGalleryProcessor
{
    public Task<Gallery> CreateGallery(Gallery gallery);

    public Task<bool> DeleteGallery(Gallery gallery);

    public Task<IEnumerable<Gallery>> GetGalleries(string galleryType);

    public Task UpdateGallery(Gallery gallery);

    public Task<bool> AddGalleryImage(ImageUpload imageUpload, Gallery gallery);

    public Task<bool> RemoveGalleryImage(GalleryImage galleryImage, Gallery gallery);
}