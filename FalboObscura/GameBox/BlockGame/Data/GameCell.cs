// ------------------------------------
// Falbo Obscura
// ------------------------------------

namespace GameBox.BlockGame.Data;

public class GameCell
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int SolvedX { get; set; }
    public int SolvedY { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}