// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Processors;

public interface IGalleryProcessor
{
    public Task CreateGalleryImage(GalleryImage galleryImage);

    public Task<bool> DeleteGalleryImage(Guid id, string imageType);

    public Task<IEnumerable<GalleryImage>> GetGalleryImages(string imageType);

    public Task UpdateGalleryImage(GalleryImage galleryImage);
}