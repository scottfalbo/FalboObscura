// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;
using FalboObscura.Core.Services;
using FalboObscura.UnitTests.Builders;
using Microsoft.JSInterop;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FalboObscura.UnitTests;

[TestClass]
public sealed class ThemeServiceTests
{
    private readonly ThemeBuilder _themeBuilder = new();
    
    private IJSRuntime _mockJSRuntime = default!;
    private ThemeService _themeService = default!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockJSRuntime = Substitute.For<IJSRuntime>();
        _themeService = new ThemeService(_mockJSRuntime);
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_WithValidJSRuntime_InitializesWithDefaults()
    {
        // Act
        var service = new ThemeService(_mockJSRuntime);

        // Assert
        Assert.IsNotNull(service);
        Assert.AreEqual("dark", service.CurrentTheme.Name);
        Assert.AreEqual("theme-dark", service.CurrentTheme.CssClass);
        Assert.AreEqual(2, service.AvailableThemes.Count());
        
        var availableThemeNames = service.AvailableThemes.Select(t => t.Name).ToList();
        CollectionAssert.Contains(availableThemeNames, "dark");
        CollectionAssert.Contains(availableThemeNames, "light");
    }

    [TestMethod]
    public void Constructor_WithNullJSRuntime_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new ThemeService(null!));
    }

    [TestMethod]
    public void AvailableThemes_ReturnsReadOnlyCollection()
    {
        // Act
        var availableThemes = _themeService.AvailableThemes;

        // Assert
        Assert.IsNotNull(availableThemes);
        Assert.IsInstanceOfType(availableThemes, typeof(IEnumerable<Theme>));
        // Verify it's read-only by checking we can't cast to List
        Assert.IsNull(availableThemes as List<Theme>);
    }

    #endregion

    #region InitializeAsync Tests

    [TestMethod]
    public async Task InitializeAsync_WithValidSavedTheme_UpdatesCurrentTheme()
    {
        // Arrange
        var savedThemeName = "light";
        _mockJSRuntime.InvokeAsync<string>("themeStorage.initializeTheme")
            .Returns(savedThemeName);

        bool themeChangedFired = false;
        Theme? changedTheme = null;
        _themeService.ThemeChanged += (sender, theme) =>
        {
            themeChangedFired = true;
            changedTheme = theme;
        };

        // Act
        await _themeService.InitializeAsync();

        // Assert
        Assert.AreEqual("light", _themeService.CurrentTheme.Name);
        Assert.IsTrue(themeChangedFired);
        Assert.IsNotNull(changedTheme);
        Assert.AreEqual("light", changedTheme.Name);

        await _mockJSRuntime.Received(1).InvokeAsync<string>("themeStorage.initializeTheme");
    }

    [TestMethod]
    public async Task InitializeAsync_WithInvalidSavedTheme_KeepsDefaultTheme()
    {
        // Arrange
        var invalidThemeName = "invalid-theme";
        _mockJSRuntime.InvokeAsync<string>("themeStorage.initializeTheme")
            .Returns(invalidThemeName);

        bool themeChangedFired = false;
        _themeService.ThemeChanged += (sender, theme) => themeChangedFired = true;

        // Act
        await _themeService.InitializeAsync();

        // Assert
        Assert.AreEqual("dark", _themeService.CurrentTheme.Name); // Should remain default
        Assert.IsFalse(themeChangedFired);

        await _mockJSRuntime.Received(1).InvokeAsync<string>("themeStorage.initializeTheme");
    }

    [TestMethod]
    public async Task InitializeAsync_WithEmptyOrNullSavedTheme_KeepsDefaultTheme()
    {
        // Arrange
        _mockJSRuntime.InvokeAsync<string>("themeStorage.initializeTheme")
            .Returns(string.Empty);

        bool themeChangedFired = false;
        _themeService.ThemeChanged += (sender, theme) => themeChangedFired = true;

        // Act
        await _themeService.InitializeAsync();

        // Assert
        Assert.AreEqual("dark", _themeService.CurrentTheme.Name); // Should remain default
        Assert.IsFalse(themeChangedFired);
    }

    [TestMethod]
    public async Task InitializeAsync_WhenJSInteropThrows_KeepsDefaultTheme()
    {
        // Arrange
        _mockJSRuntime.InvokeAsync<string>("themeStorage.initializeTheme")
            .Throws(new JSException("JS Error"));

        bool themeChangedFired = false;
        _themeService.ThemeChanged += (sender, theme) => themeChangedFired = true;

        // Act & Assert - Should not throw
        await _themeService.InitializeAsync();

        Assert.AreEqual("dark", _themeService.CurrentTheme.Name); // Should remain default
        Assert.IsFalse(themeChangedFired);
    }

    #endregion

    #region SetThemeAsync Tests

    [TestMethod]
    public async Task SetThemeAsync_WithValidThemeName_UpdatesCurrentTheme()
    {
        // Arrange
        var newThemeName = "light";
        
        bool themeChangedFired = false;
        Theme? changedTheme = null;
        _themeService.ThemeChanged += (sender, theme) =>
        {
            themeChangedFired = true;
            changedTheme = theme;
        };

        // Act
        await _themeService.SetThemeAsync(newThemeName);

        // Assert
        Assert.AreEqual("light", _themeService.CurrentTheme.Name);
        Assert.IsTrue(themeChangedFired);
        Assert.IsNotNull(changedTheme);
        Assert.AreEqual("light", changedTheme.Name);

        // Verify JS interop was called (we can't easily verify specific calls due to extension method limitations)
        Assert.AreEqual(2, _mockJSRuntime.ReceivedCalls().Count());
    }

    [TestMethod]
    public async Task SetThemeAsync_WithCaseInsensitiveThemeName_UpdatesCurrentTheme()
    {
        // Arrange
        var newThemeName = "LIGHT"; // Different case
        
        bool themeChangedFired = false;
        _themeService.ThemeChanged += (sender, theme) => themeChangedFired = true;

        // Act
        await _themeService.SetThemeAsync(newThemeName);

        // Assert
        Assert.AreEqual("light", _themeService.CurrentTheme.Name); // Should match actual theme name
        Assert.IsTrue(themeChangedFired);
        
        // Verify JS interop was called
        Assert.AreEqual(2, _mockJSRuntime.ReceivedCalls().Count());
    }

    [TestMethod]
    public async Task SetThemeAsync_WithInvalidThemeName_DoesNotUpdateTheme()
    {
        // Arrange
        var invalidThemeName = "invalid-theme";
        var originalTheme = _themeService.CurrentTheme.Name;
        
        bool themeChangedFired = false;
        _themeService.ThemeChanged += (sender, theme) => themeChangedFired = true;

        // Act
        await _themeService.SetThemeAsync(invalidThemeName);

        // Assert
        Assert.AreEqual(originalTheme, _themeService.CurrentTheme.Name);
        Assert.IsFalse(themeChangedFired);

        // Verify no JS interop calls were made
        Assert.AreEqual(0, _mockJSRuntime.ReceivedCalls().Count());
    }

    [TestMethod]
    public async Task SetThemeAsync_WithSameTheme_DoesNotTriggerChange()
    {
        // Arrange - Current theme is already "dark"
        var sameThemeName = "dark";
        
        bool themeChangedFired = false;
        _themeService.ThemeChanged += (sender, theme) => themeChangedFired = true;

        // Act
        await _themeService.SetThemeAsync(sameThemeName);

        // Assert
        Assert.AreEqual("dark", _themeService.CurrentTheme.Name);
        Assert.IsFalse(themeChangedFired);

        // Verify no JS interop calls were made
        Assert.AreEqual(0, _mockJSRuntime.ReceivedCalls().Count());
    }

    [TestMethod]
    public async Task SetThemeAsync_WhenJSInteropThrows_StillUpdatesThemeAndFiresEvent()
    {
        // Arrange
        var newThemeName = "light";
        
        // Setup JS runtime to throw exception when any InvokeAsync is called
        _mockJSRuntime
            .When(x => x.InvokeAsync<object>("themeStorage.setTheme", Arg.Any<object[]>()))
            .Do(x => throw new JSException("JS Error"));
        
        bool themeChangedFired = false;
        _themeService.ThemeChanged += (sender, theme) => themeChangedFired = true;

        // Act & Assert - Should not throw
        await _themeService.SetThemeAsync(newThemeName);

        Assert.AreEqual("light", _themeService.CurrentTheme.Name);
        Assert.IsTrue(themeChangedFired);
    }

    #endregion

    #region GetThemeAsync Tests

    [TestMethod]
    public async Task GetThemeAsync_ReturnsCurrentThemeName()
    {
        // Act
        var result = await _themeService.GetThemeAsync();

        // Assert
        Assert.AreEqual(_themeService.CurrentTheme.Name, result);
        Assert.AreEqual("dark", result); // Default theme
    }

    [TestMethod]
    public async Task GetThemeAsync_AfterThemeChange_ReturnsNewThemeName()
    {
        // Arrange
        await _themeService.SetThemeAsync("light");

        // Act
        var result = await _themeService.GetThemeAsync();

        // Assert
        Assert.AreEqual("light", result);
    }

    #endregion

    #region GetThemeCssClass Tests

    [TestMethod]
    public void GetThemeCssClass_ReturnsCurrentThemeCssClass()
    {
        // Act
        var result = _themeService.GetThemeCssClass();

        // Assert
        Assert.AreEqual(_themeService.CurrentTheme.CssClass, result);
        Assert.AreEqual("theme-dark", result); // Default theme CSS class
    }

    [TestMethod]
    public async Task GetThemeCssClass_AfterThemeChange_ReturnsNewThemeCssClass()
    {
        // Arrange
        await _themeService.SetThemeAsync("light");

        // Act
        var result = _themeService.GetThemeCssClass();

        // Assert
        Assert.AreEqual("theme-light", result);
    }

    #endregion

    #region RegisterTheme Tests

    [TestMethod]
    public void RegisterTheme_WithNewTheme_AddsToAvailableThemes()
    {
        // Arrange
        var newTheme = _themeBuilder
            .WithName("custom")
            .WithDisplayName("Custom Theme")
            .WithCssClass("theme-custom")
            .BuildTheme();

        var initialCount = _themeService.AvailableThemes.Count();

        // Act
        _themeService.RegisterTheme(newTheme);

        // Assert
        Assert.AreEqual(initialCount + 1, _themeService.AvailableThemes.Count());
        Assert.IsTrue(_themeService.AvailableThemes.Any(t => t.Name == "custom"));
    }

    [TestMethod]
    public void RegisterTheme_WithExistingThemeName_DoesNotAddDuplicate()
    {
        // Arrange
        var duplicateTheme = _themeBuilder
            .WithName("dark") // Same as existing theme
            .WithDisplayName("Another Dark Theme")
            .BuildTheme();

        var initialCount = _themeService.AvailableThemes.Count();

        // Act
        _themeService.RegisterTheme(duplicateTheme);

        // Assert
        Assert.AreEqual(initialCount, _themeService.AvailableThemes.Count());
    }

    [TestMethod]
    public async Task RegisterTheme_ThenSetTheme_AllowsUsingNewTheme()
    {
        // Arrange
        var newTheme = _themeBuilder
            .WithName("custom")
            .WithDisplayName("Custom Theme")
            .WithCssClass("theme-custom")
            .BuildTheme();

        _themeService.RegisterTheme(newTheme);

        bool themeChangedFired = false;
        _themeService.ThemeChanged += (sender, theme) => themeChangedFired = true;

        // Act
        await _themeService.SetThemeAsync("custom");

        // Assert
        Assert.AreEqual("custom", _themeService.CurrentTheme.Name);
        Assert.AreEqual("theme-custom", _themeService.CurrentTheme.CssClass);
        Assert.IsTrue(themeChangedFired);
    }

    #endregion

    #region Event Tests

    [TestMethod]
    public async Task ThemeChanged_WithMultipleSubscribers_NotifiesAllSubscribers()
    {
        // Arrange
        int eventFireCount = 0;
        Theme? receivedTheme1 = null;
        Theme? receivedTheme2 = null;

        _themeService.ThemeChanged += (sender, theme) =>
        {
            eventFireCount++;
            receivedTheme1 = theme;
        };

        _themeService.ThemeChanged += (sender, theme) =>
        {
            eventFireCount++;
            receivedTheme2 = theme;
        };

        // Act
        await _themeService.SetThemeAsync("light");

        // Assert
        Assert.AreEqual(2, eventFireCount);
        Assert.IsNotNull(receivedTheme1);
        Assert.IsNotNull(receivedTheme2);
        Assert.AreEqual("light", receivedTheme1.Name);
        Assert.AreEqual("light", receivedTheme2.Name);
    }

    [TestMethod]
    public async Task ThemeChanged_WithNoSubscribers_DoesNotThrow()
    {
        // Arrange - No event subscribers

        // Act & Assert - Should not throw
        await _themeService.SetThemeAsync("light");

        Assert.AreEqual("light", _themeService.CurrentTheme.Name);
    }

    #endregion
}