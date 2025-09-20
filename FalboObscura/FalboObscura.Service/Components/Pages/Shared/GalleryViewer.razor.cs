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

    protected override async Task OnInitializedAsync()
    {
        // TODO: Implement image querying logic based on ImageType
        await Task.CompletedTask;
    }

    private async Task HandleButtonClick()
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
    }
}