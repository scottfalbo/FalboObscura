// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Processors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace FalboObscura.Components.Pages.Shared;

public partial class GalleryViewer : ComponentBase
{
    private bool _hasRendered = false;

    public IEnumerable<Gallery> Galleries { get; set; } = [];

    [Inject]
    public IGalleryProcessor? GalleryProcessor { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState>? AuthStateTask { get; set; }

    private bool IsAuthenticated { get; set; } = false;

    [Parameter]
    public string GalleryType { get; set; } = string.Empty;

    private bool IsLoading { get; set; } = false;

    private string LoadingMessage { get; set; } = "Loading...";

    private Gallery NewGalleryModel { get; set; } = new();

    // Single upload model - tracks which gallery via SelectedGalleryId
    private ImageUpload UploadModel { get; set; } = new();
    private Guid? SelectedGalleryId { get; set; }
    private List<IBrowserFile> SelectedFiles { get; set; } = [];
    private const int MaxFileCount = 10;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    // Drag and drop state for galleries
    private Gallery? DraggedGallery { get; set; }

    // Drag and drop state for images
    private GalleryImage? DraggedImage { get; set; }
    private Guid? DraggedImageGalleryId { get; set; }

    // Lightbox carousel state
    private bool IsLightboxOpen { get; set; } = false;
    private Gallery? LightboxGallery { get; set; }
    private int LightboxCurrentIndex { get; set; } = 0;
    private List<GalleryImage> LightboxImages { get; set; } = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_hasRendered)
        {
            _hasRendered = true;
            
            // Check authentication state
            if (AuthStateTask != null)
            {
                var authState = await AuthStateTask;
                IsAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
            }
            
            await LoadGalleries();
        }
    }

    private async Task LoadGalleries()
    {
        if (GalleryProcessor != null && !string.IsNullOrEmpty(GalleryType))
        {
            Galleries = await GalleryProcessor.GetGalleries(GalleryType);
            StateHasChanged();
        }
    }

    private void StartAddImage(Gallery gallery)
    {
        SelectedGalleryId = gallery.Id;
        UploadModel = new ImageUpload { GalleryType = GalleryType };
        StateHasChanged();
    }

    private void CancelAddImage()
    {
        SelectedGalleryId = null;
        UploadModel = new ImageUpload();
        SelectedFiles = [];
        StateHasChanged();
    }

    private async Task HandleCreateGallery()
    {
        if (GalleryProcessor == null || string.IsNullOrEmpty(NewGalleryModel.GalleryName)) return;

        IsLoading = true;
        LoadingMessage = "Creating gallery...";
        StateHasChanged();

        try
        {
            NewGalleryModel.GalleryType = GalleryType;
            NewGalleryModel.GalleryImages = new List<GalleryImage>();
            NewGalleryModel.Order = Galleries.Any() ? Galleries.Max(g => g.Order) + 1 : 0;
            await GalleryProcessor.CreateGallery(NewGalleryModel);

            NewGalleryModel = new Gallery();
            await LoadGalleries();
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task HandleDeleteGallery(Gallery gallery)
    {
        if (GalleryProcessor == null) return;

        IsLoading = true;
        LoadingMessage = "Deleting gallery...";
        StateHasChanged();

        try
        {
            await GalleryProcessor.DeleteGallery(gallery);

            // Reorder remaining galleries
            var remainingGalleries = Galleries.Where(g => g.Id != gallery.Id).OrderBy(g => g.Order).ToList();
            for (int i = 0; i < remainingGalleries.Count; i++)
            {
                if (remainingGalleries[i].Order != i)
                {
                    remainingGalleries[i].Order = i;
                    await GalleryProcessor.UpdateGallery(remainingGalleries[i]);
                }
            }

            await LoadGalleries();
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task HandleAddImage()
    {
        if (GalleryProcessor == null || SelectedGalleryId == null || !SelectedFiles.Any()) return;

        var gallery = Galleries.FirstOrDefault(g => g.Id == SelectedGalleryId);
        if (gallery == null) return;

        IsLoading = true;
        var totalFiles = SelectedFiles.Count;
        var currentFile = 0;
        StateHasChanged();

        try
        {
            foreach (var file in SelectedFiles)
            {
                currentFile++;
                LoadingMessage = $"Uploading image {currentFile} of {totalFiles}...";
                StateHasChanged();

                var uploadModel = new ImageUpload
                {
                    GalleryType = GalleryType,
                    ImageFile = file,
                    AltText = UploadModel.AltText,
                    Title = UploadModel.Title,
                    Description = UploadModel.Description
                };

                await GalleryProcessor.AddGalleryImage(uploadModel, gallery);

                // Refresh gallery to get updated image list for next iteration
                var updatedGalleries = await GalleryProcessor.GetGalleries(GalleryType);
                gallery = updatedGalleries.FirstOrDefault(g => g.Id == SelectedGalleryId);
                if (gallery == null) break;
            }

            SelectedGalleryId = null;
            UploadModel = new ImageUpload();
            SelectedFiles = [];
            await LoadGalleries();
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task HandleDeleteImage(GalleryImage image, Gallery gallery)
    {
        if (GalleryProcessor == null) return;

        IsLoading = true;
        LoadingMessage = "Deleting image...";
        StateHasChanged();

        try
        {
            await GalleryProcessor.RemoveGalleryImage(image, gallery);
            await LoadGalleries();
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private void HandleFileSelected(InputFileChangeEventArgs e)
    {
        SelectedFiles = [];

        var files = e.GetMultipleFiles(MaxFileCount);
        foreach (var file in files)
        {
            if (file.Size > MaxFileSize)
            {
                Console.WriteLine($"File '{file.Name}' too large: {file.Size} bytes. Max allowed: {MaxFileSize} bytes. Skipping.");
                continue;
            }
            SelectedFiles.Add(file);
        }

        // For single file, also set on UploadModel for backward compatibility
        if (SelectedFiles.Count == 1)
        {
            UploadModel.ImageFile = SelectedFiles[0];
        }

        StateHasChanged();
    }

    private void HandleDragStart(Gallery gallery)
    {
        DraggedGallery = gallery;
    }

    private void HandleDragEnd()
    {
        DraggedGallery = null;
    }

    private async Task HandleDrop(Gallery targetGallery)
    {
        if (DraggedGallery == null || DraggedGallery.Id == targetGallery.Id || GalleryProcessor == null) return;

        var galleriesList = Galleries.OrderBy(g => g.Order).ToList();
        var draggedIndex = galleriesList.FindIndex(g => g.Id == DraggedGallery.Id);
        var targetIndex = galleriesList.FindIndex(g => g.Id == targetGallery.Id);

        if (draggedIndex < 0 || targetIndex < 0) return;

        // Remove dragged item and insert at target position
        var dragged = galleriesList[draggedIndex];
        galleriesList.RemoveAt(draggedIndex);
        galleriesList.Insert(targetIndex, dragged);

        // Update order values
        for (int i = 0; i < galleriesList.Count; i++)
        {
            galleriesList[i].Order = i;
        }

        // Persist changes
        IsLoading = true;
        LoadingMessage = "Reordering galleries...";
        StateHasChanged();

        try
        {
            foreach (var gallery in galleriesList)
            {
                await GalleryProcessor.UpdateGallery(gallery);
            }
            await LoadGalleries();
        }
        finally
        {
            DraggedGallery = null;
            IsLoading = false;
            StateHasChanged();
        }
    }

    private void HandleImageDragStart(GalleryImage image, Gallery gallery)
    {
        DraggedImage = image;
        DraggedImageGalleryId = gallery.Id;
    }

    private void HandleImageDragEnd()
    {
        DraggedImage = null;
        DraggedImageGalleryId = null;
    }

    private async Task HandleImageDrop(GalleryImage targetImage, Gallery gallery)
    {
        if (DraggedImage == null || DraggedImage.Id == targetImage.Id || GalleryProcessor == null) return;
        if (DraggedImageGalleryId != gallery.Id) return; // Only allow reordering within same gallery

        var imagesList = gallery.GalleryImages.OrderBy(i => i.Order).ToList();
        var draggedIndex = imagesList.FindIndex(i => i.Id == DraggedImage.Id);
        var targetIndex = imagesList.FindIndex(i => i.Id == targetImage.Id);

        if (draggedIndex < 0 || targetIndex < 0) return;

        // Remove dragged item and insert at target position
        var dragged = imagesList[draggedIndex];
        imagesList.RemoveAt(draggedIndex);
        imagesList.Insert(targetIndex, dragged);

        // Update order values
        for (int i = 0; i < imagesList.Count; i++)
        {
            imagesList[i].Order = i;
        }

        // Update the gallery's image list
        gallery.GalleryImages = imagesList;

        // Persist changes
        IsLoading = true;
        LoadingMessage = "Reordering images...";
        StateHasChanged();

        try
        {
            await GalleryProcessor.UpdateGallery(gallery);
            await LoadGalleries();
        }
        finally
        {
            DraggedImage = null;
            DraggedImageGalleryId = null;
            IsLoading = false;
            StateHasChanged();
        }
    }

    private void OpenLightbox(GalleryImage image, Gallery gallery)
    {
        LightboxGallery = gallery;
        LightboxImages = gallery.GalleryImages.OrderBy(i => i.Order).ToList();
        LightboxCurrentIndex = LightboxImages.FindIndex(i => i.Id == image.Id);
        if (LightboxCurrentIndex < 0) LightboxCurrentIndex = 0;
        IsLightboxOpen = true;
        StateHasChanged();
    }

    private void CloseLightbox()
    {
        IsLightboxOpen = false;
        LightboxGallery = null;
        LightboxImages = [];
        LightboxCurrentIndex = 0;
        StateHasChanged();
    }

    private void LightboxPrevious()
    {
        if (LightboxImages.Count == 0) return;
        LightboxCurrentIndex = (LightboxCurrentIndex - 1 + LightboxImages.Count) % LightboxImages.Count;
        StateHasChanged();
    }

    private void LightboxNext()
    {
        if (LightboxImages.Count == 0) return;
        LightboxCurrentIndex = (LightboxCurrentIndex + 1) % LightboxImages.Count;
        StateHasChanged();
    }

    private void LightboxGoToIndex(int index)
    {
        if (index >= 0 && index < LightboxImages.Count)
        {
            LightboxCurrentIndex = index;
            StateHasChanged();
        }
    }
}