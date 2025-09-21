// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Clients;
using FalboObscura.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace FalboObscura.Core.Processors;

public class BlobStorageProcessor(IBlobStorageClient client) : IBlobStorageProcessor
{
    private const string ContainerName = "falbo-obscura";
    private const int MaxDisplayHeight = 1080;
    private const int MaxDisplayWidth = 1920;
    private const long MaxFileSize = 10 * 1024 * 1024; //10mb
    private const int ThumbnailHeight = 300;
    private const int ThumbnailWidth = 400;

    private readonly IBlobStorageClient _client = client;

    public async Task<string> StoreImage(ImageUpload imageUpload)
    {
        var image = imageUpload.ImageFile ?? throw new ArgumentException("ImageFile cannot be null", nameof(imageUpload));

        var baseFilename = $"{imageUpload.ImageType}-{imageUpload.Id}";
        var thumbnailFilename = $"{baseFilename}-thumbnail";

        using var originalStream = image.OpenReadStream(MaxFileSize);

        using var displayImageStream = ResizeImage(originalStream, MaxDisplayWidth, MaxDisplayHeight, 85);
        var displayBlobUrl = await _client.UploadBlobAsync(ContainerName, baseFilename, displayImageStream, "image/jpeg");

        originalStream.Position = 0;

        using var thumbnailStream = ResizeImage(originalStream, ThumbnailWidth, ThumbnailHeight, 75);
        await _client.UploadBlobAsync(ContainerName, thumbnailFilename, thumbnailStream, "image/jpeg");

        return displayBlobUrl;
    }

    private static MemoryStream ResizeImage(Stream inputStream, int maxWidth, int maxHeight, int quality = 85)
    {
        var outputStream = new MemoryStream();

        using var image = Image.Load(inputStream);

        // Calculate new dimensions while preserving aspect ratio
        var ratioX = (double)maxWidth / image.Width;
        var ratioY = (double)maxHeight / image.Height;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);

        // Resize the image
        image.Mutate(x => x.Resize(newWidth, newHeight));

        // Save as JPEG with specified quality
        var encoder = new JpegEncoder { Quality = quality };
        image.Save(outputStream, encoder);

        outputStream.Position = 0;
        return outputStream;
    }
}