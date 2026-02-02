// ------------------------------------
// Falbo Obscura - Plague Doctor
// ------------------------------------

namespace GameBox.PlagueDoctor.Data;

public class PlagueService
{
    private readonly Random _random = new();

    public BoardGrid Board { get; private set; } = new();
    public GameState State { get; private set; } = new();
    public Pill? CurrentPill { get; private set; }
    public Pill? NextPill { get; private set; }

    public event Action? OnStateChanged;

    public void StartNewGame(int level = 0)
    {
        Board.Clear();
        State.Reset(level);
        PlaceViruses();
        State.VirusesRemaining = Board.CountViruses();
        SpawnNextPill();
        SpawnNextPill();  // Call twice to populate both current and next
        State.StartLevel();
        NotifyStateChanged();
    }

    public void StartNextLevel()
    {
        Board.Clear();
        State.Level++;
        State.Status = GameStatus.NotStarted;
        PlaceViruses();
        State.VirusesRemaining = Board.CountViruses();
        SpawnNextPill();
        SpawnNextPill();
        State.StartLevel();
        NotifyStateChanged();
    }

    private void PlaceViruses()
    {
        var colors = Enum.GetValues<PillColor>();
        int placed = 0;
        int attempts = 0;
        int maxAttempts = 1000;

        while (placed < State.VirusCount && attempts < maxAttempts)
        {
            int row = _random.Next(State.VirusStartRow, BoardGrid.Rows);
            int col = _random.Next(0, BoardGrid.Columns);

            if (Board.IsCellEmpty(row, col))
            {
                var color = colors[_random.Next(colors.Length)];
                Board.PlaceVirus(row, col, color);
                placed++;
            }
            attempts++;
        }
    }

    private void SpawnNextPill()
    {
        CurrentPill = NextPill;
        NextPill = Pill.CreateRandom(_random, BoardGrid.Columns / 2 - 1);

        // Check if spawn position is blocked = game over
        if (CurrentPill != null && !Board.CanPlacePill(CurrentPill))
        {
            State.SetGameOver();
        }
    }

    public bool MoveLeft()
    {
        if (CurrentPill == null || State.Status != GameStatus.Playing) return false;

        var newPill = ClonePill(CurrentPill);
        newPill.Y--;

        if (CanMove(newPill))
        {
            CurrentPill.Y--;
            NotifyStateChanged();
            return true;
        }
        return false;
    }

    public bool MoveRight()
    {
        if (CurrentPill == null || State.Status != GameStatus.Playing) return false;

        var newPill = ClonePill(CurrentPill);
        newPill.Y++;

        if (CanMove(newPill))
        {
            CurrentPill.Y++;
            NotifyStateChanged();
            return true;
        }
        return false;
    }

    public bool MoveDown()
    {
        if (CurrentPill == null || State.Status != GameStatus.Playing) return false;

        var newPill = ClonePill(CurrentPill);
        newPill.X++;

        if (CanMove(newPill))
        {
            CurrentPill.X++;
            NotifyStateChanged();
            return true;
        }
        else
        {
            // Can't move down - lock the pill
            LockPill();
            return false;
        }
    }

    public void HardDrop()
    {
        if (CurrentPill == null || State.Status != GameStatus.Playing) return;

        while (MoveDown()) { }
    }

    public bool RotateClockwise()
    {
        if (CurrentPill == null || State.Status != GameStatus.Playing) return false;

        var newPill = ClonePill(CurrentPill);
        newPill.RotateClockwise();

        if (CanMove(newPill))
        {
            CurrentPill.RotateClockwise();
            NotifyStateChanged();
            return true;
        }

        // Try wall kick - move left or right if rotation is blocked
        newPill.Y--;
        if (CanMove(newPill))
        {
            CurrentPill.Y--;
            CurrentPill.RotateClockwise();
            NotifyStateChanged();
            return true;
        }

        newPill.Y += 2;
        if (CanMove(newPill))
        {
            CurrentPill.Y++;
            CurrentPill.RotateClockwise();
            NotifyStateChanged();
            return true;
        }

        return false;
    }

    public bool RotateCounterClockwise()
    {
        if (CurrentPill == null || State.Status != GameStatus.Playing) return false;

        var newPill = ClonePill(CurrentPill);
        newPill.RotateCounterClockwise();

        if (CanMove(newPill))
        {
            CurrentPill.RotateCounterClockwise();
            NotifyStateChanged();
            return true;
        }

        return false;
    }

    private bool CanMove(Pill pill)
    {
        var (x2, y2) = pill.SecondHalfPosition;

        return Board.IsValidPosition(pill.X, pill.Y) &&
               Board.IsValidPosition(x2, y2) &&
               Board.IsCellEmpty(pill.X, pill.Y) &&
               Board.IsCellEmpty(x2, y2);
    }

    private void LockPill()
    {
        if (CurrentPill == null) return;

        Board.PlacePill(CurrentPill);
        ProcessMatches();

        State.CheckVictory(Board.CountViruses());

        if (State.Status == GameStatus.Playing)
        {
            SpawnNextPill();
        }

        NotifyStateChanged();
    }

    private void ProcessMatches()
    {
        int comboMultiplier = 1;

        while (true)
        {
            var matches = Board.FindMatches();
            if (matches.Count == 0) break;

            int virusesCleared = Board.ClearMatches(matches);
            State.AddScore(virusesCleared, comboMultiplier);

            // Apply gravity and check for chain reactions
            while (Board.ApplyGravity()) { }

            comboMultiplier++;
        }
    }

    public void Tick()
    {
        if (State.Status == GameStatus.Playing)
        {
            MoveDown();
        }
    }

    public void Pause()
    {
        if (State.Status == GameStatus.Playing)
            State.Status = GameStatus.Paused;
    }

    public void Resume()
    {
        if (State.Status == GameStatus.Paused)
            State.Status = GameStatus.Playing;
    }

    private Pill ClonePill(Pill pill)
    {
        return new Pill
        {
            X = pill.X,
            Y = pill.Y,
            Color1 = pill.Color1,
            Color2 = pill.Color2,
            Orientation = pill.Orientation
        };
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}
