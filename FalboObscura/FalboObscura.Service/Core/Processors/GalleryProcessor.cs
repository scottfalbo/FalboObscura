// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Repositories;

namespace FalboObscura.Core.Processors;

public class GalleryProcessor(
    IImageRepository imageRepository,
    IBlobStorageProcessor blobStorageProcessor) : IGalleryProcessor
{
    private readonly IBlobStorageProcessor _blobStorageProcessor = blobStorageProcessor ?? throw new ArgumentNullException(nameof(blobStorageProcessor));
    private readonly IImageRepository _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
}