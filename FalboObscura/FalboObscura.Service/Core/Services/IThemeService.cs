// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.Core.Services;

public interface IThemeService
{
    IEnumerable<Theme> AvailableThemes { get; }

    Theme CurrentTheme { get; }

    Task<string> GetThemeAsync();

    string GetThemeCssClass();

    Task InitializeAsync();

    Task SetThemeAsync(string themeName);

    event EventHandler<Theme>? ThemeChanged;
}