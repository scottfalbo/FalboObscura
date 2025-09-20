// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Processors;

public interface IGalleryProcessor
{
    public Task CreateGalleryImage(GalleryImage galleryImage);

    public Task DeleteGalleryImage(Guid id);

    public Task<IEnumerable<GalleryImage>> GetGalleryImages(string imageType);

    public Task UpdateGalleryImage(GalleryImage galleryImage);
}