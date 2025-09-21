// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Models.Constants;
using FalboObscura.Core.Models.StorageContracts;

namespace FalboObscura.UnitTests.Builders;

internal class GalleryImageBuilder
{
    private string _altText = "image alt text";
    private string? _description;
    private Guid _id = Guid.NewGuid();
    private string _imageThumbnailUrl = "test-thumb-url";
    private string _imageType = ImageType.TattooImage;
    private string _imageUrl = "test-url";
    private string? _title;

    public GalleryImage BuildGalleryImage()
    {
        var galleryImage = new GalleryImage()
        {
            AltText = _altText,
            Description = _description,
            Id = _id,
            ImageThumbnailUrl = _imageThumbnailUrl,
            ImageType = _imageType,
            ImageUrl = _imageUrl,
            Title = _title
        };

        return galleryImage;
    }

    public GalleryImageStorageContract BuildGalleryImageStorageContract()
    {
        var galleryImage = new GalleryImageStorageContract()
        {
            AltText = _altText,
            Description = _description,
            Id = _id,
            ImageThumbnailUrl = _imageThumbnailUrl,
            ImageType = _imageType,
            ImageUrl = _imageUrl,
            PartitionKey = _imageType,
            Title = _title
        };

        return galleryImage;
    }

    public GalleryImageBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public GalleryImageBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public GalleryImageBuilder WithImageType(string imageType)
    {
        _imageType = imageType;
        return this;
    }
}