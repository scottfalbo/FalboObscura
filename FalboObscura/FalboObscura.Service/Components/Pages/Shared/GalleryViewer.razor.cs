// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Microsoft.AspNetCore.Components;

namespace FalboObscura.Components.Pages.Shared;

public partial class GalleryViewer : ComponentBase
{
    [Parameter] public string ImageType { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        // TODO: Implement image querying logic based on ImageType
        await Task.CompletedTask;
    }
}