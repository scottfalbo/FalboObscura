// ------------------------------------
// Falbo Obscura
// ------------------------------------

using GameBox.BlockGame.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace GameBox.BlockGame;
#nullable disable

public partial class GameBoard : ComponentBase
{
    [Inject] private GameService GameService { get; set; }
    [Inject] private LoadImageService LoadImageService { get; set; }

    private Puzzle Puzzle;
    private int X = 7;
    private int Y = 4;
    private GameCell RemovedCell;
    private bool Winner;
    private int NumberOfMoves;

    protected override void OnInitialized()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (GameService == null) return;
        
        Winner = false;
        NumberOfMoves = 0;
        Puzzle = GameService.CreateBoard(X, Y);
        RemovedCell = Puzzle.Rows[X - 1].Cells[Y - 1];
        Puzzle.Rows[X - 1].Cells[Y - 1] = null;
        Puzzle = GameService.RandomizeBoard(Puzzle, X, Y);
    }

    public void ClickCell(int x, int y)
    {
        if (!Winner)
        {
            Coords move = GameService.AvailableMove(Puzzle!, x, y);

            if (move != null)
            {
                Puzzle.Rows[move.X].Cells[move.Y] = Puzzle.Rows[x].Cells[y];
                Puzzle.Rows[move.X].Cells[move.Y].X = move.X;
                Puzzle.Rows[move.X].Cells[move.Y].Y = move.Y;
                Puzzle.Rows[x].Cells[y] = null;
                NumberOfMoves++;
            }

            Winner = GameService.CheckWinner(Puzzle!);
            if (Winner) Puzzle.Rows[Puzzle.X - 1].Cells[Puzzle.Y - 1] = RemovedCell;
        }
    }

    private async Task LoadFile(InputFileChangeEventArgs e)
    {
        IBrowserFile file = e.File;
    }
}

#nullable enable