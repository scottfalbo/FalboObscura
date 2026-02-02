// ------------------------------------
// Falbo Obscura
// ------------------------------------

namespace GameBox.BlockGame.Data;

public class GameService
{
    /// <summary>
    /// Create a new game board with a size of x, y
    /// </summary>
    /// <param name="x"> int height </param>
    /// <param name="y"> int width </param>
    /// <returns> populated Puzzle object </returns>
    public Puzzle CreateBoard(int x, int y) => new(x, y);

    /// <summary>
    /// Randomize the cells on the game board.
    /// </summary>
    /// <param name="board"> GameCell[,] </param>
    /// <param name="x"> int for x index random </param>
    /// <param name="y"> int for y index random </param>
    /// <returns> randomized GameCel[,] </returns>
    public Puzzle RandomizeBoard(Puzzle board, int x, int y)
    {
        Random random = new();
        for (int i = 0; i < 100; i++)
        {
            int x1 = random.Next(0, x);
            int x2 = random.Next(0, x);
            int y1 = random.Next(0, y);
            int y2 = random.Next(0, y);

            GameCell holder = board.Rows[x1].Cells[y1];
            if (holder != null)
            {
                holder.X = x2;
                holder.Y = y2;
            }
            board.Rows[x1].Cells[y1] = board.Rows[x2].Cells[y2];
            if (board.Rows[x1].Cells[y1] != null)
            {
                board.Rows[x1].Cells[y1].X = x1;
                board.Rows[x1].Cells[y1].Y = y1;
            }
            board.Rows[x2].Cells[y2] = holder!;
        }
        return board;
    }

    /// <summary>
    /// Check around the clicked cell for available moves, return coords of move or null
    /// </summary>
    /// <param name="board"> Puzzle object </param>
    /// <param name="x"> clicked cell x </param>
    /// <param name="y"> clicked cell y </param>
    /// <returns> Coords object with move : null </returns>
    public Coords? AvailableMove(Puzzle board, int x, int y)
    {
        if (x - 1 >= 0)
        {
            if (board.Rows[x - 1].Cells[y] == null)
                return new Coords(x - 1, y);
        }
        if (x + 1 < board.X)
        {
            if (board.Rows[x + 1].Cells[y] == null)
                return new Coords(x + 1, y);
        }
        if (y - 1 >= 0)
        {
            if (board.Rows[x].Cells[y - 1] == null)
                return new Coords(x, y - 1);
        }
        if (y + 1 < board.Y)
        {
            if (board.Rows[x].Cells[y + 1] == null)
                return new Coords(x, y + 1);
        }
        return null;
    }

    /// <summary>
    /// Checks to see if the puzzle in complete.
    /// </summary>
    /// <param name="board"> Puzzle object </param>
    /// <returns> true : false </returns>
    public bool CheckWinner(Puzzle board)
    {
        bool winner = true;

        foreach (Row rows in board.Rows)
        {
            foreach (GameCell cell in rows.Cells)
            {
                if (cell != null)
                {
                    if (cell.X != cell.SolvedX || cell.Y != cell.SolvedY)
                        winner = false;
                }
            }
        }
        return winner;
    }
}