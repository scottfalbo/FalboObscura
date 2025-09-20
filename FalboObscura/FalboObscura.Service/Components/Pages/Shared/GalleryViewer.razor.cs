// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Processors;
using Microsoft.AspNetCore.Components;

namespace FalboObscura.Components.Pages.Shared;

public partial class GalleryViewer : ComponentBase
{
    [Inject]
    public IGalleryProcessor? GalleryProcessor { get; set; }

    [Parameter] public string ImageType { get; set; } = string.Empty;

    public IEnumerable<GalleryImage> GalleryImages { get; set; } = [];

    private string DeleteImageId = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (GalleryProcessor != null && !string.IsNullOrEmpty(ImageType))
        {
            GalleryImages = await GalleryProcessor.GetGalleryImages(ImageType);
        }
    }

    private async Task CreateGalleryImage()
    {
        // TODO: Implement your logic here
        var galleryImage = new GalleryImage()
        {
            AltText = "test",
            Description = "test",
            ImageThumbnailUrl = "test",
            ImageType = ImageType,
            ImageUrl = "test"
        };

        await GalleryProcessor!.CreateGalleryImage(galleryImage);
        
        if (GalleryProcessor != null && !string.IsNullOrEmpty(ImageType))
        {
            GalleryImages = await GalleryProcessor.GetGalleryImages(ImageType);
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
}