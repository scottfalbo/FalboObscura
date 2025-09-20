// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Models.Constants;

namespace FalboObscura.Core.Processors;

public interface IGalleryProcessor
{
    public void CreateGalleryImage();

    public IEnumerable<GalleryImage> GetGalleryImages(ImageType imageType);
}