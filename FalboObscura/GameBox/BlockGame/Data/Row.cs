// ------------------------------------
// Falbo Obscura
// ------------------------------------

namespace GameBox.BlockGame.Data;

public class Row(int y)
{
    public GameCell[] Cells { get; set; } = new GameCell[y];
}