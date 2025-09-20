// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Repositories;

public interface IGalleryImageRepository
{
    Task CreateGalleryImage(GalleryImage galleryImage);

    Task DeleteGalleryImage(Guid id, string partitionKey);

    Task<IEnumerable<GalleryImage>> GetGalleryImages(string partitionKey);

    Task<GalleryImage> UpdateGalleryImage(GalleryImage galleryImage);
}