// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core;
using FalboObscura.Core.Models;
using FalboObscura.Core.Models.StorageContracts;
using FalboObscura.UnitTests.Builders;

namespace FalboObscura.UnitTests;

[TestClass]
public sealed class MapperTests
{
    private readonly GalleryImageBuilder _galleryImageBuilder = new();
    private readonly GalleryImageMapper _galleryImageMapper = new();

    [TestMethod]
    public void GalleryImage_DomainModelToStorageContract_WithNullablesAssigned_MapsCorrectly()
    {
        var galleryImage = _galleryImageBuilder
            .WithTitle("test-title")
            .WithDescription("test-desc")
            .BuildGalleryImage();

        var galleryImageStorageContract = _galleryImageMapper.DomainModelToStorageContract(galleryImage);

        GalleryImageAsserts(galleryImage, galleryImageStorageContract);
    }

    [TestMethod]
    public void GalleryImage_DomainModelToStorageContract_WithNullProperties_MapsCorrectly()
    {
        var galleryImage = _galleryImageBuilder.BuildGalleryImage();

        var galleryImageStorageContract = _galleryImageMapper.DomainModelToStorageContract(galleryImage);

        GalleryImageAsserts(galleryImage, galleryImageStorageContract);
    }

    [TestMethod]
    public void GalleryImage_StorageContractToDomainModel_WithNullablesAssigned_MapsCorrectly()
    {
        var galleryImageStorageContract = _galleryImageBuilder
            .WithTitle("test-title")
            .WithDescription("test-desc")
            .BuildGalleryImageStorageContract();

        var galleryImage = _galleryImageMapper.StorageContractToDomainModel(galleryImageStorageContract);

        GalleryImageAsserts(galleryImage, galleryImageStorageContract);
    }

    [TestMethod]
    public void GalleryImage_StorageContractToDomainModel_WithNullProperties_MapsCorrectly()
    {
        var galleryImageStorageContract = _galleryImageBuilder.BuildGalleryImageStorageContract();

        var galleryImage = _galleryImageMapper.StorageContractToDomainModel(galleryImageStorageContract);

        GalleryImageAsserts(galleryImage, galleryImageStorageContract);
    }

    private static void GalleryImageAsserts(GalleryImage galleryImage, GalleryImageStorageContract galleryImageStorageContract)
    {
        Assert.AreEqual(galleryImage.AltText, galleryImageStorageContract.AltText);
        Assert.AreEqual(galleryImage.Description, galleryImageStorageContract.Description);
        Assert.AreEqual(galleryImage.Id, galleryImageStorageContract.Id);
        Assert.AreEqual(galleryImage.ImageThumbnailUrl, galleryImageStorageContract.ImageThumbnailUrl);
        Assert.AreEqual(galleryImage.ImageType, galleryImageStorageContract.ImageType);
        Assert.AreEqual(galleryImage.ImageUrl, galleryImageStorageContract.ImageUrl);
        Assert.AreEqual(galleryImage.ImageType, galleryImageStorageContract.PartitionKey);
        Assert.AreEqual(galleryImage.Title, galleryImageStorageContract.Title);
    }
}