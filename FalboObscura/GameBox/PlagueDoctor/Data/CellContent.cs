// ------------------------------------
// Falbo Obscura - Plague Doctor
// ------------------------------------

namespace GameBox.PlagueDoctor.Data;

public enum PillColor
{
    Orange,
    Lavender,
    Mint
}

public enum CellContentType
{
    Empty,
    Virus,
    PillHalf
}

public class CellContent
{
    public CellContentType Type { get; set; } = CellContentType.Empty;
    public PillColor? Color { get; set; }
    public bool IsConnectedLeft { get; set; }
    public bool IsConnectedRight { get; set; }
    public bool IsConnectedUp { get; set; }
    public bool IsConnectedDown { get; set; }
    public bool MarkedForClear { get; set; }

    public static CellContent Empty() => new() { Type = CellContentType.Empty };

    public static CellContent CreateVirus(PillColor color) => new()
    {
        Type = CellContentType.Virus,
        Color = color
    };

    public static CellContent CreatePillHalf(PillColor color) => new()
    {
        Type = CellContentType.PillHalf,
        Color = color
    };

    public bool IsEmpty => Type == CellContentType.Empty;
    public bool IsVirus => Type == CellContentType.Virus;
    public bool IsPillHalf => Type == CellContentType.PillHalf;
}
