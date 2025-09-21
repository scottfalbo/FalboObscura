// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Models.Constants;
using FalboObscura.Core.Processors;
using FalboObscura.Core.Repositories;
using FalboObscura.UnitTests.Builders;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FalboObscura.UnitTests;

[TestClass]
public sealed class GalleryProcessorTests
{
    private readonly GalleryImageBuilder _galleryImageBuilder = new();
    private readonly ImageUploadBuilder _imageUploadBuilder = new();

    private GalleryProcessor _galleryProcessor = default!;
    private IBlobStorageProcessor _mockBlobStorageProcessor = default!;
    private IGalleryImageRepository _mockGalleryImageRepository = default!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockGalleryImageRepository = Substitute.For<IGalleryImageRepository>();
        _mockBlobStorageProcessor = Substitute.For<IBlobStorageProcessor>();
        _galleryProcessor = new GalleryProcessor(_mockGalleryImageRepository, _mockBlobStorageProcessor);
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_WithNullBlobStorageProcessor_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new GalleryProcessor(_mockGalleryImageRepository, null!));
    }

    [TestMethod]
    public void Constructor_WithNullGalleryImageRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new GalleryProcessor(null!, _mockBlobStorageProcessor));
    }

    [TestMethod]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Act
        var processor = new GalleryProcessor(_mockGalleryImageRepository, _mockBlobStorageProcessor);

        // Assert
        Assert.IsNotNull(processor);
    }

    #endregion Constructor Tests

    #region CreateGalleryImage Tests

    [TestMethod]
    public async Task CreateGalleryImage_WhenBlobStorageThrowsException_ThrowsException()
    {
        // Arrange
        var imageUpload = _imageUploadBuilder.BuildImageUpload();
        var expectedException = new InvalidOperationException("Blob storage error");

        _mockBlobStorageProcessor.StoreImage(imageUpload)
            .Throws(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<Exception>(() =>
            _galleryProcessor.CreateGalleryImage(imageUpload));

        Assert.AreEqual("womp womp", exception.Message);
        Assert.AreEqual(expectedException, exception.InnerException);

        await _mockBlobStorageProcessor.Received(1).StoreImage(imageUpload);
        await _mockGalleryImageRepository.DidNotReceive().CreateGalleryImage(Arg.Any<GalleryImage>());
    }

    [TestMethod]
    public async Task CreateGalleryImage_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var imageUpload = _imageUploadBuilder.BuildImageUpload();
        var expectedBlobUrl = "https://test.blob.url/image.jpg";
        var expectedException = new InvalidOperationException("Repository error");

        _mockBlobStorageProcessor.StoreImage(imageUpload)
            .Returns(expectedBlobUrl);

        _mockGalleryImageRepository.CreateGalleryImage(Arg.Any<GalleryImage>())
            .Throws(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<Exception>(() =>
            _galleryProcessor.CreateGalleryImage(imageUpload));

        Assert.AreEqual("womp womp", exception.Message);
        Assert.AreEqual(expectedException, exception.InnerException);

        await _mockBlobStorageProcessor.Received(1).StoreImage(imageUpload);
        await _mockGalleryImageRepository.Received(1).CreateGalleryImage(Arg.Any<GalleryImage>());
    }

    [TestMethod]
    public async Task CreateGalleryImage_WithMinimalImageUpload_ReturnsGalleryImage()
    {
        // Arrange
        var imageUpload = _imageUploadBuilder
            .WithAltText("Test Alt Text")
            .WithImageType(ImageType.DrawingImage)
            .WithDescription(null)
            .WithTitle(null)
            .BuildImageUpload();

        var expectedBlobUrl = "https://test.blob.url/image.jpg";

        _mockBlobStorageProcessor.StoreImage(imageUpload)
            .Returns(expectedBlobUrl);

        // Act
        var result = await _galleryProcessor.CreateGalleryImage(imageUpload);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Title);
        Assert.IsNull(result.Description);
        Assert.AreEqual(imageUpload.AltText, result.AltText);
        Assert.AreEqual(imageUpload.ImageType, result.ImageType);
        Assert.AreEqual(expectedBlobUrl, result.ImageUrl);
        Assert.AreEqual($"{expectedBlobUrl}-thumbnail", result.ImageThumbnailUrl);
    }

    [TestMethod]
    public async Task CreateGalleryImage_WithValidImageUpload_ReturnsGalleryImage()
    {
        // Arrange
        var imageUpload = _imageUploadBuilder
            .WithTitle("Test Title")
            .WithDescription("Test Description")
            .WithAltText("Test Alt Text")
            .WithImageType(ImageType.TattooImage)
            .BuildImageUpload();

        var expectedBlobUrl = "https://test.blob.url/image.jpg";

        _mockBlobStorageProcessor.StoreImage(imageUpload)
            .Returns(expectedBlobUrl);

        // Act
        var result = await _galleryProcessor.CreateGalleryImage(imageUpload);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(imageUpload.Title, result.Title);
        Assert.AreEqual(imageUpload.Description, result.Description);
        Assert.AreEqual(imageUpload.AltText, result.AltText);
        Assert.AreEqual(imageUpload.ImageType, result.ImageType);
        Assert.AreEqual(expectedBlobUrl, result.ImageUrl);
        Assert.AreEqual($"{expectedBlobUrl}-thumbnail", result.ImageThumbnailUrl);
        Assert.AreEqual(imageUpload.Id, result.Id);

        await _mockBlobStorageProcessor.Received(1).StoreImage(imageUpload);
        await _mockGalleryImageRepository.Received(1).CreateGalleryImage(Arg.Any<GalleryImage>());
    }

    #endregion CreateGalleryImage Tests

    #region DeleteGalleryImage Tests

    [TestMethod]
    public async Task DeleteGalleryImage_WhenBlobStorageThrowsException_ReturnsFalse()
    {
        // Arrange
        var galleryId = Guid.NewGuid();
        var imageType = ImageType.OtherArt;

        _mockBlobStorageProcessor.DeleteImage(galleryId, imageType)
            .Throws(new InvalidOperationException("Blob storage error"));

        // Act
        var result = await _galleryProcessor.DeleteGalleryImage(galleryId, imageType);

        // Assert
        Assert.IsFalse(result);
        await _mockGalleryImageRepository.Received(1).DeleteGalleryImage(galleryId, imageType);
        await _mockBlobStorageProcessor.Received(1).DeleteImage(galleryId, imageType);
    }

    [TestMethod]
    public async Task DeleteGalleryImage_WhenRepositoryThrowsException_ReturnsFalse()
    {
        // Arrange
        var galleryId = Guid.NewGuid();
        var imageType = ImageType.DrawingImage;

        _mockGalleryImageRepository.DeleteGalleryImage(galleryId, imageType)
            .Throws(new InvalidOperationException("Repository error"));

        // Act
        var result = await _galleryProcessor.DeleteGalleryImage(galleryId, imageType);

        // Assert
        Assert.IsFalse(result);
        await _mockGalleryImageRepository.Received(1).DeleteGalleryImage(galleryId, imageType);
        await _mockBlobStorageProcessor.DidNotReceive().DeleteImage(Arg.Any<Guid>(), Arg.Any<string>());
    }

    [TestMethod]
    public async Task DeleteGalleryImage_WithValidIdAndType_ReturnsTrue()
    {
        // Arrange
        var galleryId = Guid.NewGuid();
        var imageType = ImageType.TattooImage;

        // Act
        var result = await _galleryProcessor.DeleteGalleryImage(galleryId, imageType);

        // Assert
        Assert.IsTrue(result);
        await _mockGalleryImageRepository.Received(1).DeleteGalleryImage(galleryId, imageType);
        await _mockBlobStorageProcessor.Received(1).DeleteImage(galleryId, imageType);
    }

    #endregion DeleteGalleryImage Tests

    #region GetGalleryImages Tests

    [TestMethod]
    public async Task GetGalleryImages_WhenRepositoryThrowsException_ReturnsEmptyCollection()
    {
        // Arrange
        var imageType = ImageType.OtherArt;

        _mockGalleryImageRepository.GetGalleryImages(imageType)
            .Throws(new InvalidOperationException("Repository error"));

        // Act
        var result = await _galleryProcessor.GetGalleryImages(imageType);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());

        await _mockGalleryImageRepository.Received(1).GetGalleryImages(imageType);
    }

    [TestMethod]
    public async Task GetGalleryImages_WithEmptyResult_ReturnsEmptyCollection()
    {
        // Arrange
        var imageType = ImageType.DrawingImage;
        var expectedGalleryImages = new List<GalleryImage>();

        _mockGalleryImageRepository.GetGalleryImages(imageType)
            .Returns(expectedGalleryImages);

        // Act
        var result = await _galleryProcessor.GetGalleryImages(imageType);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());

        await _mockGalleryImageRepository.Received(1).GetGalleryImages(imageType);
    }

    [TestMethod]
    public async Task GetGalleryImages_WithValidImageType_ReturnsGalleryImages()
    {
        // Arrange
        var imageType = ImageType.TattooImage;
        var expectedGalleryImages = new List<GalleryImage>
        {
            _galleryImageBuilder.WithImageType(imageType).BuildGalleryImage(),
            _galleryImageBuilder.WithImageType(imageType).BuildGalleryImage()
        };

        _mockGalleryImageRepository.GetGalleryImages(imageType)
            .Returns(expectedGalleryImages);

        // Act
        var result = await _galleryProcessor.GetGalleryImages(imageType);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedGalleryImages.Count, result.Count());
        CollectionAssert.AreEqual(expectedGalleryImages.ToArray(), result.ToArray());

        await _mockGalleryImageRepository.Received(1).GetGalleryImages(imageType);
    }

    #endregion GetGalleryImages Tests

    #region UpdateGalleryImage Tests

    [TestMethod]
    public async Task UpdateGalleryImage_WhenRepositoryThrowsException_DoesNotThrow()
    {
        // Arrange
        var galleryImage = _galleryImageBuilder.BuildGalleryImage();

        _mockGalleryImageRepository.UpdateGalleryImage(galleryImage)
            .Throws(new InvalidOperationException("Repository error"));

        // Act & Assert - Should not throw exception
        await _galleryProcessor.UpdateGalleryImage(galleryImage);

        await _mockGalleryImageRepository.Received(1).UpdateGalleryImage(galleryImage);
    }

    [TestMethod]
    public async Task UpdateGalleryImage_WithValidGalleryImage_CallsRepository()
    {
        // Arrange
        var galleryImage = _galleryImageBuilder
            .WithTitle("Updated Title")
            .WithDescription("Updated Description")
            .BuildGalleryImage();

        // Act
        await _galleryProcessor.UpdateGalleryImage(galleryImage);

        // Assert
        await _mockGalleryImageRepository.Received(1).UpdateGalleryImage(galleryImage);
    }

    #endregion UpdateGalleryImage Tests
}