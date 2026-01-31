// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Repositories;

public interface IGalleryRepository
{
    Task CreateGallery(Gallery gallery);

    Task DeleteGallery(Guid id, string partitionKey);

    Task<IEnumerable<Gallery>> GetGalleries(string partitionKey);

    Task<Gallery> UpdateGallery(Gallery gallery);
}