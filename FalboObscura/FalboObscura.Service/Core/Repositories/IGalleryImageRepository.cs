// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Repositories;

public interface IGalleryImageRepository
{
    Task<IEnumerable<GalleryImage>> GetGalleryImages(string partitionKey);
    Task<GalleryImage> Create(GalleryImage galleryImage);
    Task<GalleryImage> Update(GalleryImage galleryImage);
}