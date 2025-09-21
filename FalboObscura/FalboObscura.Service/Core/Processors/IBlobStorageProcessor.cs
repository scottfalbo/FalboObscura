// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Processors;

public interface IBlobStorageProcessor
{
    public Task DeleteImage(Guid id, string imageType);

    public Task<string> StoreImage(ImageUpload imageUpload);
}