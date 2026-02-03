// ------------------------------------
// Falbo Obscura - Plague Doctor
// ------------------------------------

namespace GameBox.PlagueDoctor.Data;

public enum GamePhase
{
    Playing,        // Normal pill control
    Clearing,       // Matches found, showing cleared cells
    Dropping,       // Gravity is being applied
    Checking        // Checking for chain reactions
}

public class PlagueService
{
    private readonly Random _random = new();

    public BoardGrid Board { get; private set; } = new();
    public GameState State { get; private set; } = new();
    public Pill? CurrentPill { get; private set; }
    public Pill? NextPill { get; private set; }
    public GamePhase Phase { get; private set; } = GamePhase.Playing;
    
    private int _comboMultiplier = 1;

    public event Action? OnStateChanged;

    public void StartNewGame(int level = 0)
    {
        State.Type = GameType.TypeA;
        Board.Clear();
        State.Reset(level);
        PlaceViruses();
        State.VirusesRemaining = Board.CountViruses();
        SpawnNextPill();
        SpawnNextPill();  // Call twice to populate both current and next
        State.StartLevel();
        NotifyStateChanged();
    }

    public void StartTypeBGame(SpeedSetting speed, int virusLevel)
    {
        State.Type = GameType.TypeB;
        State.SelectedSpeed = speed;
        State.SelectedVirusLevel = virusLevel;
        Board.Clear();
        State.Reset(0);
        PlaceViruses();
        State.VirusesRemaining = Board.CountViruses();
        SpawnNextPill();
        SpawnNextPill();
        State.StartLevel();
        NotifyStateChanged();
    }

    public void StartNextLevel()
    {
        // Type B doesn't have next level - game ends on victory
        if (State.Type == GameType.TypeB)
        {
            return;
        }
        
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

        // DEBUG: Always rotate to test if the swap works
        CurrentPill.RotateClockwise();
        NotifyStateChanged();
        return true;
    }

    public bool RotateClockwise_WithCollision()
    {
        if (CurrentPill == null || State.Status != GameStatus.Playing) return false;

        var testPill = ClonePill(CurrentPill);
        testPill.RotateClockwise();

        // Try rotation in place
        if (CanMove(testPill))
        {
            CurrentPill.RotateClockwise();
            NotifyStateChanged();
            return true;
        }

        // Try wall kick LEFT
        testPill = ClonePill(CurrentPill);
        testPill.RotateClockwise();
        testPill.Y--;
        if (CanMove(testPill))
        {
            CurrentPill.Y--;
            CurrentPill.RotateClockwise();
            NotifyStateChanged();
            return true;
        }

        // Try wall kick RIGHT
        testPill = ClonePill(CurrentPill);
        testPill.RotateClockwise();
        testPill.Y++;
        if (CanMove(testPill))
        {
            CurrentPill.Y++;
            CurrentPill.RotateClockwise();
            NotifyStateChanged();
            return true;
        }

        // Try floor kick UP (for rotating near bottom or when cell below is occupied)
        testPill = ClonePill(CurrentPill);
        testPill.RotateClockwise();
        testPill.X--;
        if (CanMove(testPill))
        {
            CurrentPill.X--;
            CurrentPill.RotateClockwise();
            NotifyStateChanged();
            return true;
        }

        return false;
    }

    public bool RotateCounterClockwise()
    {
        if (CurrentPill == null || State.Status != GameStatus.Playing) return false;

        var testPill = ClonePill(CurrentPill);
        testPill.RotateCounterClockwise();

        // Try rotation in place
        if (CanMove(testPill))
        {
            CurrentPill.RotateCounterClockwise();
            NotifyStateChanged();
            return true;
        }

        // Try wall kick LEFT
        testPill = ClonePill(CurrentPill);
        testPill.RotateCounterClockwise();
        testPill.Y--;
        if (CanMove(testPill))
        {
            CurrentPill.Y--;
            CurrentPill.RotateCounterClockwise();
            NotifyStateChanged();
            return true;
        }

        // Try wall kick RIGHT
        testPill = ClonePill(CurrentPill);
        testPill.RotateCounterClockwise();
        testPill.Y++;
        if (CanMove(testPill))
        {
            CurrentPill.Y++;
            CurrentPill.RotateCounterClockwise();
            NotifyStateChanged();
            return true;
        }

        // Try floor kick UP
        testPill = ClonePill(CurrentPill);
        testPill.RotateCounterClockwise();
        testPill.X--;
        if (CanMove(testPill))
        {
            CurrentPill.X--;
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
        CurrentPill = null;  // Clear current pill while processing
        _comboMultiplier = 1;
        
        // Start the match/clear/drop cycle
        StartMatchPhase();
    }

    private void StartMatchPhase()
    {
        var matches = Board.FindMatches();
        if (matches.Count > 0)
        {
            int virusesCleared = Board.ClearMatches(matches);
            State.AddScore(virusesCleared, _comboMultiplier);
            _comboMultiplier++;
            Phase = GamePhase.Dropping;
            NotifyStateChanged();
        }
        else
        {
            // No matches, finish up and spawn next pill
            FinishTurn();
        }
    }

    /// <summary>
    /// Called by the game loop timer during Dropping phase to animate gravity
    /// </summary>
    public void ProcessDropping()
    {
        if (Phase != GamePhase.Dropping) return;

        bool anythingFell = Board.ApplyGravity();
        NotifyStateChanged();

        if (!anythingFell)
        {
            // Nothing left to fall, check for chain reactions
            Phase = GamePhase.Checking;
            StartMatchPhase();  // Check for new matches after gravity
        }
    }

    private void FinishTurn()
    {
        Phase = GamePhase.Playing;
        State.CheckVictory(Board.CountViruses());

        if (State.Status == GameStatus.Playing)
        {
            SpawnNextPill();
        }

        NotifyStateChanged();
    }

    public void Tick()
    {
        if (State.Status != GameStatus.Playing) return;

        if (Phase == GamePhase.Dropping)
        {
            ProcessDropping();
        }
        else if (Phase == GamePhase.Playing)
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
