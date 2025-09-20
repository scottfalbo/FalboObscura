// ------------------------------------
// Falbo Obscura
// ------------------------------------

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
}