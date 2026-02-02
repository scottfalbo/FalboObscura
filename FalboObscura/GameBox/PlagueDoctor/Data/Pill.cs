// ------------------------------------
// Falbo Obscura - Plague Doctor
// ------------------------------------

namespace GameBox.PlagueDoctor.Data;

public enum PillOrientation
{
    Horizontal,  // Left-Right
    Vertical     // Top-Bottom
}

public class Pill
{
    public int X { get; set; }  // Row position of first half
    public int Y { get; set; }  // Column position of first half
    public PillColor Color1 { get; set; }  // First half color
    public PillColor Color2 { get; set; }  // Second half color
    public PillOrientation Orientation { get; set; } = PillOrientation.Horizontal;

    /// <summary>
    /// Gets the position of the second half based on orientation
    /// </summary>
    public (int X, int Y) SecondHalfPosition => Orientation switch
    {
        PillOrientation.Horizontal => (X, Y + 1),
        PillOrientation.Vertical => (X + 1, Y),
        _ => (X, Y + 1)
    };

    public static Pill CreateRandom(Random random, int startY = 3)
    {
        var colors = Enum.GetValues<PillColor>();
        return new Pill
        {
            X = 0,  // Start at top
            Y = startY,  // Center of board
            Color1 = colors[random.Next(colors.Length)],
            Color2 = colors[random.Next(colors.Length)],
            Orientation = PillOrientation.Horizontal
        };
    }

    public void RotateClockwise()
    {
        Orientation = Orientation switch
        {
            PillOrientation.Horizontal => PillOrientation.Vertical,
            PillOrientation.Vertical => PillOrientation.Horizontal,
            _ => PillOrientation.Horizontal
        };
        // Swap colors on rotation to maintain visual consistency
        (Color1, Color2) = (Color2, Color1);
    }

    public void RotateCounterClockwise()
    {
        Orientation = Orientation switch
        {
            PillOrientation.Horizontal => PillOrientation.Vertical,
            PillOrientation.Vertical => PillOrientation.Horizontal,
            _ => PillOrientation.Horizontal
        };
    }
}
