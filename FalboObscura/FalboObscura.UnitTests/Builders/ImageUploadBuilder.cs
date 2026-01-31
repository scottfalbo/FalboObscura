// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Models.Constants;
using Microsoft.AspNetCore.Components.Forms;

namespace FalboObscura.UnitTests.Builders;

internal class ImageUploadBuilder
{
    private string _altText = "image alt text";
    private string? _description;
    private Guid _id = Guid.NewGuid();
    private IBrowserFile? _imageFile;
    private string _imageType = GalleryType.TattooGallery;
    private string? _title;

    public ImageUpload BuildImageUpload()
    {
        return new ImageUpload()
        {
            AltText = _altText,
            Description = _description,
            Id = _id,
            ImageFile = _imageFile,
            ImageType = _imageType,
            Title = _title,
        };
    }

    public ImageUploadBuilder WithAltText(string altText)
    {
        _altText = altText;
        return this;
    }

    public ImageUploadBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public ImageUploadBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ImageUploadBuilder WithImageFile(IBrowserFile? imageFile)
    {
        _imageFile = imageFile;
        return this;
    }

    public ImageUploadBuilder WithImageType(string imageType)
    {
        _imageType = imageType;
        return this;
    }

    public ImageUploadBuilder WithTitle(string? title)
    {
        _title = title;
        return this;
    }
}