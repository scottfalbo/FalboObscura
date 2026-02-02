// ------------------------------------
// Falbo Obscura - Plague Doctor
// ------------------------------------

namespace GameBox.PlagueDoctor.Data;

public enum PillOrientation
{
    Horizontal0,   // Color1 left, Color2 right
    Vertical0,     // Color1 top, Color2 bottom
    Horizontal1,   // Color1 right, Color2 left (flipped)
    Vertical1      // Color1 bottom, Color2 top (flipped)
}

public class Pill
{
    public int X { get; set; }  // Row position of anchor
    public int Y { get; set; }  // Column position of anchor
    public PillColor Color1 { get; set; }  // First color (doesn't change)
    public PillColor Color2 { get; set; }  // Second color (doesn't change)
    public PillOrientation Orientation { get; set; } = PillOrientation.Horizontal0;

    /// <summary>
    /// Gets the position of the second half based on orientation
    /// </summary>
    public (int X, int Y) SecondHalfPosition => Orientation switch
    {
        PillOrientation.Horizontal0 => (X, Y + 1),
        PillOrientation.Vertical0 => (X + 1, Y),
        PillOrientation.Horizontal1 => (X, Y + 1),
        PillOrientation.Vertical1 => (X + 1, Y),
        _ => (X, Y + 1)
    };

    /// <summary>
    /// Gets which color is at the anchor position (X, Y)
    /// </summary>
    public PillColor AnchorColor => Orientation switch
    {
        PillOrientation.Horizontal0 => Color1,  // Color1 on left
        PillOrientation.Vertical0 => Color1,    // Color1 on top
        PillOrientation.Horizontal1 => Color2,  // Color2 on left
        PillOrientation.Vertical1 => Color2,    // Color2 on top
        _ => Color1
    };

    /// <summary>
    /// Gets which color is at the second position
    /// </summary>
    public PillColor SecondColor => Orientation switch
    {
        PillOrientation.Horizontal0 => Color2,  // Color2 on right
        PillOrientation.Vertical0 => Color2,    // Color2 on bottom
        PillOrientation.Horizontal1 => Color1,  // Color1 on right
        PillOrientation.Vertical1 => Color1,    // Color1 on bottom
        _ => Color2
    };

    public bool IsHorizontal => Orientation == PillOrientation.Horizontal0 || Orientation == PillOrientation.Horizontal1;
    public bool IsVertical => Orientation == PillOrientation.Vertical0 || Orientation == PillOrientation.Vertical1;

    public static Pill CreateRandom(Random random, int startY = 3)
    {
        var colors = Enum.GetValues<PillColor>();
        return new Pill
        {
            X = 0,  // Start at top
            Y = startY,  // Center of board
            Color1 = colors[random.Next(colors.Length)],
            Color2 = colors[random.Next(colors.Length)],
            Orientation = PillOrientation.Horizontal0
        };
    }

    public void RotateClockwise()
    {
        Orientation = Orientation switch
        {
            PillOrientation.Horizontal0 => PillOrientation.Vertical0,
            PillOrientation.Vertical0 => PillOrientation.Horizontal1,
            PillOrientation.Horizontal1 => PillOrientation.Vertical1,
            PillOrientation.Vertical1 => PillOrientation.Horizontal0,
            _ => PillOrientation.Horizontal0
        };
    }

    public void RotateCounterClockwise()
    {
        Orientation = Orientation switch
        {
            PillOrientation.Horizontal0 => PillOrientation.Vertical1,
            PillOrientation.Vertical1 => PillOrientation.Horizontal1,
            PillOrientation.Horizontal1 => PillOrientation.Vertical0,
            PillOrientation.Vertical0 => PillOrientation.Horizontal0,
            _ => PillOrientation.Horizontal0
        };
    }
}
