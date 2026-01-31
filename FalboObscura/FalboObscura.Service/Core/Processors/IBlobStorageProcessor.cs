// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Processors;

public interface IBlobStorageProcessor
{
    public Task DeleteImage(Guid id);

    public Task<string> StoreImage(ImageUpload imageUpload);
}