// ------------------------------------
// Falbo Obscura - Plague Doctor
// ------------------------------------

namespace GameBox.PlagueDoctor.Data;

public class BoardGrid
{
    public const int Rows = 16;
    public const int Columns = 8;

    public CellContent[,] Cells { get; private set; }

    public BoardGrid()
    {
        Cells = new CellContent[Rows, Columns];
        Clear();
    }

    public void Clear()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Cells[row, col] = CellContent.Empty();
            }
        }
    }

    public CellContent GetCell(int row, int col)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Columns)
            return null!;
        return Cells[row, col];
    }

    public void SetCell(int row, int col, CellContent content)
    {
        if (row >= 0 && row < Rows && col >= 0 && col < Columns)
        {
            Cells[row, col] = content;
        }
    }

    public bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Columns;
    }

    public bool IsCellEmpty(int row, int col)
    {
        if (!IsValidPosition(row, col)) return false;
        return Cells[row, col].IsEmpty;
    }

    public bool CanPlacePill(Pill pill)
    {
        var (x2, y2) = pill.SecondHalfPosition;
        return IsCellEmpty(pill.X, pill.Y) && IsCellEmpty(x2, y2);
    }

    public void PlacePill(Pill pill)
    {
        var (x2, y2) = pill.SecondHalfPosition;

        var half1 = CellContent.CreatePillHalf(pill.Color1);
        var half2 = CellContent.CreatePillHalf(pill.Color2);

        // Set connection flags based on orientation
        if (pill.Orientation == PillOrientation.Horizontal)
        {
            half1.IsConnectedRight = true;
            half2.IsConnectedLeft = true;
        }
        else
        {
            half1.IsConnectedDown = true;
            half2.IsConnectedUp = true;
        }

        SetCell(pill.X, pill.Y, half1);
        SetCell(x2, y2, half2);
    }

    public void PlaceVirus(int row, int col, PillColor color)
    {
        SetCell(row, col, CellContent.CreateVirus(color));
    }

    /// <summary>
    /// Check for matches of 4+ same color in a row/column
    /// </summary>
    public List<(int Row, int Col)> FindMatches()
    {
        var matches = new HashSet<(int, int)>();

        // Check horizontal matches
        for (int row = 0; row < Rows; row++)
        {
            int matchStart = 0;
            int matchLength = 1;
            PillColor? currentColor = Cells[row, 0].Color;

            for (int col = 1; col < Columns; col++)
            {
                var cell = Cells[row, col];
                if (!cell.IsEmpty && cell.Color == currentColor)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 4 && currentColor.HasValue)
                    {
                        for (int i = matchStart; i < matchStart + matchLength; i++)
                        {
                            matches.Add((row, i));
                        }
                    }
                    matchStart = col;
                    matchLength = 1;
                    currentColor = cell.IsEmpty ? null : cell.Color;
                }
            }

            if (matchLength >= 4 && currentColor.HasValue)
            {
                for (int i = matchStart; i < matchStart + matchLength; i++)
                {
                    matches.Add((row, i));
                }
            }
        }

        // Check vertical matches
        for (int col = 0; col < Columns; col++)
        {
            int matchStart = 0;
            int matchLength = 1;
            PillColor? currentColor = Cells[0, col].Color;

            for (int row = 1; row < Rows; row++)
            {
                var cell = Cells[row, col];
                if (!cell.IsEmpty && cell.Color == currentColor)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 4 && currentColor.HasValue)
                    {
                        for (int i = matchStart; i < matchStart + matchLength; i++)
                        {
                            matches.Add((i, col));
                        }
                    }
                    matchStart = row;
                    matchLength = 1;
                    currentColor = cell.IsEmpty ? null : cell.Color;
                }
            }

            if (matchLength >= 4 && currentColor.HasValue)
            {
                for (int i = matchStart; i < matchStart + matchLength; i++)
                {
                    matches.Add((i, col));
                }
            }
        }

        return matches.ToList();
    }

    /// <summary>
    /// Clear matched cells and break pill connections
    /// </summary>
    public int ClearMatches(List<(int Row, int Col)> matches)
    {
        int virusesCleared = 0;

        foreach (var (row, col) in matches)
        {
            var cell = Cells[row, col];
            if (cell.IsVirus) virusesCleared++;

            // Break connections to adjacent pill halves
            BreakConnections(row, col);

            Cells[row, col] = CellContent.Empty();
        }

        return virusesCleared;
    }

    private void BreakConnections(int row, int col)
    {
        var cell = Cells[row, col];

        if (cell.IsConnectedLeft && IsValidPosition(row, col - 1))
            Cells[row, col - 1].IsConnectedRight = false;

        if (cell.IsConnectedRight && IsValidPosition(row, col + 1))
            Cells[row, col + 1].IsConnectedLeft = false;

        if (cell.IsConnectedUp && IsValidPosition(row - 1, col))
            Cells[row - 1, col].IsConnectedDown = false;

        if (cell.IsConnectedDown && IsValidPosition(row + 1, col))
            Cells[row + 1, col].IsConnectedUp = false;
    }

    /// <summary>
    /// Apply gravity - returns true if anything fell
    /// </summary>
    public bool ApplyGravity()
    {
        bool anythingFell = false;

        // Process from bottom to top, skip bottom row
        for (int row = Rows - 2; row >= 0; row--)
        {
            for (int col = 0; col < Columns; col++)
            {
                var cell = Cells[row, col];
                if (cell.IsPillHalf && !HasConnection(row, col) && IsCellEmpty(row + 1, col))
                {
                    // Single unconnected pill half falls
                    Cells[row + 1, col] = cell;
                    Cells[row, col] = CellContent.Empty();
                    anythingFell = true;
                }
            }
        }

        return anythingFell;
    }

    private bool HasConnection(int row, int col)
    {
        var cell = Cells[row, col];
        return cell.IsConnectedLeft || cell.IsConnectedRight ||
               cell.IsConnectedUp || cell.IsConnectedDown;
    }

    public int CountViruses()
    {
        int count = 0;
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (Cells[row, col].IsVirus) count++;
            }
        }
        return count;
    }
}
