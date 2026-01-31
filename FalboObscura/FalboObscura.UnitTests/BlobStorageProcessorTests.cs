// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Clients;
using FalboObscura.Core.Models.Constants;
using FalboObscura.Core.Processors;
using FalboObscura.UnitTests.Builders;
using Microsoft.AspNetCore.Components.Forms;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace FalboObscura.UnitTests;

[TestClass]
public sealed class BlobStorageProcessorTests
{
    private readonly ImageUploadBuilder _imageUploadBuilder = new();

    private IBlobStorageClient _mockBlobStorageClient = default!;
    private BlobStorageProcessor _blobStorageProcessor = default!;

    private const string ContainerName = "falbo-obscura";

    [TestInitialize]
    public void TestInitialize()
    {
        _mockBlobStorageClient = Substitute.For<IBlobStorageClient>();
        _blobStorageProcessor = new BlobStorageProcessor(_mockBlobStorageClient);
    }

    #region Helper Methods

    private static IBrowserFile CreateMockBrowserFile(string fileName = "test-image.jpg", string contentType = "image/jpeg", long size = 1024)
    {
        var mockFile = Substitute.For<IBrowserFile>();
        mockFile.Name.Returns(fileName);
        mockFile.ContentType.Returns(contentType);
        mockFile.Size.Returns(size);

        // Create a simple test image stream (minimal JPEG header + data)
        var imageBytes = CreateTestImageBytes();
        var stream = new MemoryStream(imageBytes);

        mockFile.OpenReadStream(Arg.Any<long>(), Arg.Any<CancellationToken>())
               .Returns(_ => new MemoryStream(imageBytes));

        return mockFile;
    }

    private static byte[] CreateTestImageBytes()
    {
        // Create a proper test image using ImageSharp to ensure compatibility
        using var image = new Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(10, 10);
        using var stream = new MemoryStream();

        // Fill with a simple pattern
        image.Mutate(x => x.BackgroundColor(Color.Red));

        // Save as JPEG
        var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
        image.SaveAsJpeg(stream, encoder);

        return stream.ToArray();
    }

    #endregion Helper Methods

    #region Constructor Tests

    [TestMethod]
    public void Constructor_WithValidBlobStorageClient_CreatesInstance()
    {
        // Act
        var processor = new BlobStorageProcessor(_mockBlobStorageClient);

        // Assert
        Assert.IsNotNull(processor);
    }

    [TestMethod]
    public void Constructor_WithNullBlobStorageClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new BlobStorageProcessor(null!));
    }

    #endregion Constructor Tests

    #region DeleteImage Tests

    [TestMethod]
    public async Task DeleteImage_WithValidIdAndImageType_CallsDeleteForBothMainAndThumbnailImages()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var imageType = GalleryType.TattooGallery;
        var expectedMainBlobName = $"{imageType}-{imageId}";
        var expectedThumbnailBlobName = $"{expectedMainBlobName}-thumbnail";

        _mockBlobStorageClient.DeleteBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<CancellationToken>())
                              .Returns(true);

        // Act
        await _blobStorageProcessor.DeleteImage(imageId, imageType);

        // Assert
        await _mockBlobStorageClient.Received(1).DeleteBlobAsync(ContainerName, expectedMainBlobName, Arg.Any<CancellationToken>());
        await _mockBlobStorageClient.Received(1).DeleteBlobAsync(ContainerName, expectedThumbnailBlobName, Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task DeleteImage_WhenClientThrowsException_PropagatesException()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var imageType = GalleryType.DrawingGallery;
        var expectedException = new InvalidOperationException("Blob storage error");

        _mockBlobStorageClient.DeleteBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<CancellationToken>())
                              .Throws(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            _blobStorageProcessor.DeleteImage(imageId, imageType));

        Assert.AreEqual(expectedException.Message, exception.Message);
    }

    [TestMethod]
    public async Task DeleteImage_WithEmptyGuid_CallsDeleteWithEmptyGuidInBlobName()
    {
        // Arrange
        var imageId = Guid.Empty;
        var imageType = GalleryType.Other;

        // Act
        await _blobStorageProcessor.DeleteImage(imageId, imageType);

        // Assert
        await _mockBlobStorageClient.Received(1).DeleteBlobAsync(ContainerName, $"{imageType}-{Guid.Empty}", Arg.Any<CancellationToken>());
        await _mockBlobStorageClient.Received(1).DeleteBlobAsync(ContainerName, $"{imageType}-{Guid.Empty}-thumbnail", Arg.Any<CancellationToken>());
    }

    #endregion DeleteImage Tests

    #region StoreImage Tests

    [TestMethod]
    public async Task StoreImage_WithValidImageUpload_ReturnsDisplayBlobUrl()
    {
        // Arrange
        var mockBrowserFile = CreateMockBrowserFile();
        var imageUpload = _imageUploadBuilder
            .WithImageFile(mockBrowserFile)
            .WithImageType(GalleryType.TattooGallery)
            .BuildImageUpload();

        var expectedDisplayUrl = "https://storage.example.com/display-image.jpg";
        var expectedThumbnailUrl = "https://storage.example.com/thumbnail-image.jpg";

        _mockBlobStorageClient.UploadBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<Stream>(), "image/jpeg", Arg.Any<CancellationToken>())
                              .Returns(expectedDisplayUrl, expectedThumbnailUrl);

        // Act
        var result = await _blobStorageProcessor.StoreImage(imageUpload);

        // Assert
        Assert.AreEqual(expectedDisplayUrl, result);

        // Verify both display and thumbnail uploads were called
        await _mockBlobStorageClient.Received(2).UploadBlobAsync(
            ContainerName,
            Arg.Any<string>(),
            Arg.Any<Stream>(),
            "image/jpeg",
            Arg.Any<CancellationToken>());

        // Verify specific blob names
        var expectedDisplayBlobName = $"{imageUpload.ImageType}-{imageUpload.Id}";
        var expectedThumbnailBlobName = $"{expectedDisplayBlobName}-thumbnail";

        await _mockBlobStorageClient.Received(1).UploadBlobAsync(
            ContainerName,
            expectedDisplayBlobName,
            Arg.Any<Stream>(),
            "image/jpeg",
            Arg.Any<CancellationToken>());

        await _mockBlobStorageClient.Received(1).UploadBlobAsync(
            ContainerName,
            expectedThumbnailBlobName,
            Arg.Any<Stream>(),
            "image/jpeg",
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task StoreImage_WithNullImageFile_ThrowsArgumentException()
    {
        // Arrange
        var imageUpload = _imageUploadBuilder
            .WithImageFile(null)
            .BuildImageUpload();

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
            _blobStorageProcessor.StoreImage(imageUpload));

        Assert.AreEqual("ImageFile cannot be null (Parameter 'imageUpload')", exception.Message);
        Assert.AreEqual("imageUpload", exception.ParamName);
    }

    [TestMethod]
    public async Task StoreImage_WithDifferentImageTypes_CreatesCorrectBlobNames()
    {
        // Arrange
        var testCases = new[]
        {
            GalleryType.TattooGallery,
            GalleryType.DrawingGallery,
            GalleryType.Other
        };

        foreach (var imageType in testCases)
        {
            // Reset the mock for each test case
            _mockBlobStorageClient.ClearReceivedCalls();

            var mockBrowserFile = CreateMockBrowserFile();
            var imageUpload = _imageUploadBuilder
                .WithImageFile(mockBrowserFile)
                .WithImageType(imageType)
                .BuildImageUpload();

            var expectedUrl = $"https://storage.example.com/{imageType}-image.jpg";
            _mockBlobStorageClient.UploadBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<Stream>(), "image/jpeg", Arg.Any<CancellationToken>())
                                  .Returns(expectedUrl);

            // Act
            await _blobStorageProcessor.StoreImage(imageUpload);

            // Assert
            var expectedDisplayBlobName = $"{imageType}-{imageUpload.Id}";
            var expectedThumbnailBlobName = $"{expectedDisplayBlobName}-thumbnail";

            await _mockBlobStorageClient.Received(1).UploadBlobAsync(
                ContainerName,
                expectedDisplayBlobName,
                Arg.Any<Stream>(),
                "image/jpeg",
                Arg.Any<CancellationToken>());

            await _mockBlobStorageClient.Received(1).UploadBlobAsync(
                ContainerName,
                expectedThumbnailBlobName,
                Arg.Any<Stream>(),
                "image/jpeg",
                Arg.Any<CancellationToken>());
        }
    }

    [TestMethod]
    public async Task StoreImage_WhenDisplayUploadFails_PropagatesException()
    {
        // Arrange
        var mockBrowserFile = CreateMockBrowserFile();
        var imageUpload = _imageUploadBuilder
            .WithImageFile(mockBrowserFile)
            .BuildImageUpload();

        var expectedException = new InvalidOperationException("Upload failed");
        _mockBlobStorageClient.UploadBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<Stream>(), "image/jpeg", Arg.Any<CancellationToken>())
                              .Throws(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            _blobStorageProcessor.StoreImage(imageUpload));

        Assert.AreEqual(expectedException.Message, exception.Message);
    }

    [TestMethod]
    public async Task StoreImage_WhenUploadFails_PropagatesException()
    {
        // Arrange
        var mockBrowserFile = CreateMockBrowserFile();
        var imageUpload = _imageUploadBuilder
            .WithImageFile(mockBrowserFile)
            .BuildImageUpload();

        var expectedException = new InvalidOperationException("Upload failed");

        // Setup: Any upload call fails
        _mockBlobStorageClient.UploadBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<Stream>(), "image/jpeg", Arg.Any<CancellationToken>())
                              .Throws(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            _blobStorageProcessor.StoreImage(imageUpload));

        Assert.AreEqual(expectedException.Message, exception.Message);
    }

    [TestMethod]
    public async Task StoreImage_WithLargeFile_ProcessesSuccessfully()
    {
        // Arrange
        var mockBrowserFile = CreateMockBrowserFile("large-image.jpg", "image/jpeg", 5 * 1024 * 1024); // 5MB
        var imageUpload = _imageUploadBuilder
            .WithImageFile(mockBrowserFile)
            .BuildImageUpload();

        var expectedUrl = "https://storage.example.com/large-image.jpg";
        _mockBlobStorageClient.UploadBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<Stream>(), "image/jpeg", Arg.Any<CancellationToken>())
                              .Returns(expectedUrl);

        // Act
        var result = await _blobStorageProcessor.StoreImage(imageUpload);

        // Assert
        Assert.AreEqual(expectedUrl, result);

        // Verify that the file was opened with the correct max size (10MB limit)
        mockBrowserFile.Received(2).OpenReadStream(10 * 1024 * 1024, Arg.Any<CancellationToken>());
    }

    #endregion StoreImage Tests

    #region Integration Tests

    [TestMethod]
    public async Task StoreImageThenDelete_EndToEndWorkflow_CompletesSuccessfully()
    {
        // Arrange
        var mockBrowserFile = CreateMockBrowserFile();
        var imageUpload = _imageUploadBuilder
            .WithImageFile(mockBrowserFile)
            .WithImageType(GalleryType.TattooGallery)
            .BuildImageUpload();

        var expectedUrl = "https://storage.example.com/test-image.jpg";
        _mockBlobStorageClient.UploadBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<Stream>(), "image/jpeg", Arg.Any<CancellationToken>())
                              .Returns(expectedUrl);
        _mockBlobStorageClient.DeleteBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<CancellationToken>())
                              .Returns(true);

        // Act - Store Image
        var storeResult = await _blobStorageProcessor.StoreImage(imageUpload);

        // Act - Delete Image
        await _blobStorageProcessor.DeleteImage(imageUpload.Id, imageUpload.ImageType);

        // Assert
        Assert.AreEqual(expectedUrl, storeResult);

        // Verify complete workflow
        await _mockBlobStorageClient.Received(2).UploadBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<Stream>(), "image/jpeg", Arg.Any<CancellationToken>());
        await _mockBlobStorageClient.Received(2).DeleteBlobAsync(ContainerName, Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    #endregion Integration Tests
}