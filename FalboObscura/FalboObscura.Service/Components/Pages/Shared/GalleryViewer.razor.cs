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

    public IEnumerable<GalleryImage> GalleryImages { get; set; } = [];

    [Inject]
    public IGalleryProcessor? GalleryProcessor { get; set; }

    [Parameter] public string ImageType { get; set; } = string.Empty;
    private bool IsUploading { get; set; } = false;

    // Image upload properties
    private ImageUpload UploadModel { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        if (GalleryProcessor != null && !string.IsNullOrEmpty(ImageType))
        {
            GalleryImages = await GalleryProcessor.GetGalleryImages(ImageType);
        }
    }

    private async Task CreateGalleryImage()
    {
        if (GalleryProcessor == null || UploadModel.ImageFile == null)
            return;

        IsUploading = true;
        StateHasChanged();

        try
        {
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // TODO: Show user-friendly error message
        }
        finally
        {
            IsUploading = false;
            StateHasChanged();
        }
    }

    private async Task HandleDeleteSubmit()
    {
        if (GalleryProcessor != null && !string.IsNullOrEmpty(DeleteImageId) && Guid.TryParse(DeleteImageId, out var imageId))
        {
            var success = await GalleryProcessor.DeleteGalleryImage(imageId, ImageType);

            if (success)
            {
                // Clear the input field
                DeleteImageId = string.Empty;

                // Refresh the gallery images after deletion
                if (!string.IsNullOrEmpty(ImageType))
                {
                    GalleryImages = await GalleryProcessor.GetGalleryImages(ImageType);
                    StateHasChanged();
                }
            }
        }
    }

    private void HandleFileSelected(InputFileChangeEventArgs e)
    {
        UploadModel.ImageFile = e.File;
        StateHasChanged();
    }
}