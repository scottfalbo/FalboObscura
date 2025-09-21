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
    private const long MaxFileSize = 10 * 1024 * 1024; //10mb

    private readonly IBlobStorageClient _client = client;

    public async Task DeleteImage(Guid id, string imageType)
    {
        var blobName = $"{imageType}-{id}";
        var thumbnailBlobName = $"{blobName}-thumbnail";

        await _client.DeleteBlobAsync(ContainerName, blobName);
        await _client.DeleteBlobAsync(ContainerName, thumbnailBlobName);
    }

    public async Task<string> StoreImage(ImageUpload imageUpload)
    {
        var image = imageUpload.ImageFile ?? throw new ArgumentException("ImageFile cannot be null", nameof(imageUpload));

        var baseFilename = $"{imageUpload.ImageType}-{imageUpload.Id}";
        var thumbnailFilename = $"{baseFilename}-thumbnail";

        using var displayStream = image.OpenReadStream(MaxFileSize);
        using var displayImageStream = await ResizeImageAsync(displayStream, 1920, 1080, 85);
        var displayBlobUrl = await _client.UploadBlobAsync(ContainerName, baseFilename, displayImageStream, "image/jpeg");

        using var thumbnailSourceStream = image.OpenReadStream(MaxFileSize);
        using var thumbnailStream = await ResizeImageAsync(thumbnailSourceStream, 400, 300, 75);
        await _client.UploadBlobAsync(ContainerName, thumbnailFilename, thumbnailStream, "image/jpeg");

        return displayBlobUrl;
    }

    private static async Task<MemoryStream> ResizeImageAsync(Stream inputStream, int maxWidth, int maxHeight, int quality = 85)
    {
        var outputStream = new MemoryStream();

        using var image = await Image.LoadAsync(inputStream);
        var ratioX = (double)maxWidth / image.Width;
        var ratioY = (double)maxHeight / image.Height;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);

        image.Mutate(x => x.Resize(newWidth, newHeight));

        var encoder = new JpegEncoder { Quality = quality };
        image.Save(outputStream, encoder);

        outputStream.Position = 0;
        return outputStream;
    }
}