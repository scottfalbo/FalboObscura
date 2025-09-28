namespace FalboObscura.Core.Models;

public class Theme
{
    public static Theme Dark => new()
    {
        Name = "dark",
        DisplayName = "Dark Mode",
        Description = "Dark theme with high contrast",
        CssClass = "theme-dark"
    };

    public static Theme Light => new()
    {
        Name = "light",
        DisplayName = "Light Mode",
        Description = "Clean light theme",
        CssClass = "theme-light"
    };

    public string CssClass { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}