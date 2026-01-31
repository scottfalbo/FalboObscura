using FalboObscura.Core.Models;
using Microsoft.JSInterop;

namespace FalboObscura.Core.Services;

public class ThemeService : IThemeService
{
    private Theme _currentTheme;
    private readonly List<Theme> _availableThemes;
    private readonly IJSRuntime _jsRuntime;

    public Theme CurrentTheme => _currentTheme;
    public IEnumerable<Theme> AvailableThemes => _availableThemes.AsReadOnly();

    public event EventHandler<Theme>? ThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));

        // Initialize with available themes - easily expandable
        _availableThemes = new List<Theme>
        {
            Theme.Dark,
            Theme.Light
            // Add more themes here in the future:
            // Theme.HighContrast,
            // Theme.Sunset,
            // Theme.Ocean,
            // etc.
        };

        // Set dark as default
        _currentTheme = Theme.Dark;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var savedTheme = await _jsRuntime.InvokeAsync<string>("themeStorage.initializeTheme");
            if (!string.IsNullOrEmpty(savedTheme))
            {
                // Update internal state to match what was applied to the DOM
                var theme = _availableThemes.FirstOrDefault(t => t.Name.Equals(savedTheme, StringComparison.OrdinalIgnoreCase));
                if (theme != null)
                {
                    _currentTheme = theme;
                    ThemeChanged?.Invoke(this, _currentTheme);
                }
            }
        }
        catch
        {
            // If JS interop fails, keep the default theme
        }
    }

    public async Task SetThemeAsync(string themeName)
    {
        var theme = _availableThemes.FirstOrDefault(t => t.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase));

        if (theme != null && theme.Name != _currentTheme.Name)
        {
            _currentTheme = theme;

            try
            {
                await _jsRuntime.InvokeVoidAsync("themeStorage.setTheme", themeName);
                await _jsRuntime.InvokeVoidAsync("themeStorage.applyThemeToBody", themeName);
            }
            catch
            {
                // If JS interop fails, continue without persistence
            }

            ThemeChanged?.Invoke(this, _currentTheme);
        }
    }

    public Task<string> GetThemeAsync()
    {
        return Task.FromResult(_currentTheme.Name);
    }

    public void RegisterTheme(Theme theme)
    {
        if (_availableThemes.All(t => t.Name != theme.Name))
        {
            _availableThemes.Add(theme);
        }
    }

    public string GetThemeCssClass()
    {
        return _currentTheme.CssClass;
    }
}