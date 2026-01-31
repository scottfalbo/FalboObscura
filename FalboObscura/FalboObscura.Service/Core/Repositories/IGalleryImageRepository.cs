// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Repositories;

public interface IGalleryImageRepository
{
    Task CreateGalleryImage(GalleryImage galleryImage);

    Task DeleteGalleryImage(Guid id, string partitionKey);

    Task<GalleryImage?> GetGalleryImage(Guid id, string partitionKey);

    Task<GalleryImage> UpdateGalleryImage(GalleryImage galleryImage);
}