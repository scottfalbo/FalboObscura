// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Processors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FalboObscura.Components.Pages.Shared;

public partial class GalleryViewer : ComponentBase
{
    private string DeleteImageId = string.Empty;

    private bool hasRendered = false;
    public IEnumerable<GalleryImage> GalleryImages { get; set; } = [];

    [Inject]
    public IGalleryProcessor? GalleryProcessor { get; set; }

    [Parameter] public string ImageType { get; set; } = string.Empty;
    private bool IsUploading { get; set; } = false;

    // Image upload properties
    private ImageUpload UploadModel { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !hasRendered)
        {
            hasRendered = true;
            if (GalleryProcessor != null && !string.IsNullOrEmpty(ImageType))
            {
                GalleryImages = await GalleryProcessor.GetGalleryImages(ImageType);
                StateHasChanged();
            }
        }
    }

    private async Task CreateGalleryImage()
    {
        if (GalleryProcessor == null || UploadModel.ImageFile == null) return;

        IsUploading = true;
        StateHasChanged();

        UploadModel.ImageType = ImageType;
        await GalleryProcessor.CreateGalleryImage(UploadModel);

        GalleryImages = await GalleryProcessor.GetGalleryImages(ImageType);

        UploadModel = new ImageUpload();

        IsUploading = false;
        StateHasChanged();
    }

    private async Task HandleDeleteSubmit()
    {
        if (GalleryProcessor != null && !string.IsNullOrEmpty(DeleteImageId) && Guid.TryParse(DeleteImageId, out var imageId))
        {
            var success = await GalleryProcessor.DeleteGalleryImage(imageId, ImageType);

            if (success)
            {
                DeleteImageId = string.Empty;

                GalleryImages = await GalleryProcessor.GetGalleryImages(ImageType);
                StateHasChanged();
            }
        }
    }

    private void HandleFileSelected(InputFileChangeEventArgs e)
    {
        const long maxFileSize = 10 * 1024 * 1024; // 10 MB

        var file = e.File;
        if (file.Size > maxFileSize)
        {
            // TODO: Show error message to user
            Console.WriteLine($"File too large: {file.Size} bytes. Max allowed: {maxFileSize} bytes");
            return;
        }

        UploadModel.ImageFile = file;
        StateHasChanged();
    }
}