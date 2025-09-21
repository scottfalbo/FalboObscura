// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Processors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

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
        await LogToConsole($"OnAfterRenderAsync - FirstRender: {firstRender}, HasRendered: {hasRendered}");

        if (firstRender && !hasRendered)
        {
            hasRendered = true;
            if (GalleryProcessor != null && !string.IsNullOrEmpty(ImageType))
            {
                await LogToConsole("Loading gallery images...");
                GalleryImages = await GalleryProcessor.GetGalleryImages(ImageType);
                await LogToConsole($"Loaded {GalleryImages.Count()} images");
                StateHasChanged();
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LogToConsole($"OnInitializedAsync - ImageType: {ImageType}");
        await LogToConsole($"AuthStateProvider available: {AuthStateProvider != null}");

        if (AuthStateProvider != null)
        {
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                await LogToConsole($"Authentication state retrieved - IsAuthenticated: {authState.User.Identity?.IsAuthenticated ?? false}");
                await LogToConsole($"User name: {authState.User.Identity?.Name ?? "null"}");
            }
            catch (Exception ex)
            {
                await LogToConsole($"Error getting auth state: {ex.Message}");
            }
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
            UploadModel.ImageType = ImageType;
            await GalleryProcessor.CreateGalleryImage(UploadModel);

            GalleryImages = await GalleryProcessor.GetGalleryImages(ImageType);

            UploadModel = new ImageUpload();
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

    private async Task LogToConsole(string message)
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("console.log", $"[GalleryViewer] {message}");
        }
        catch
        {
            // Ignore JS errors during logging
        }
    }
}