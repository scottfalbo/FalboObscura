// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Models;

namespace FalboObscura.UnitTests.Builders;

internal class ThemeBuilder
{
    private string _name = "test-theme";
    private string _displayName = "Test Theme";
    private string _description = "A test theme for unit testing";
    private string _cssClass = "theme-test";

    public Theme BuildTheme()
    {
        return new Theme
        {
            Name = _name,
            DisplayName = _displayName,
            Description = _description,
            CssClass = _cssClass
        };
    }

    public ThemeBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ThemeBuilder WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    public ThemeBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ThemeBuilder WithCssClass(string cssClass)
    {
        _cssClass = cssClass;
        return this;
    }
}