// ------------------------------------
// Falbo Obscura - Plague Doctor
// ------------------------------------

namespace GameBox.PlagueDoctor.Data;

public enum GameStatus
{
    NotStarted,
    Playing,
    Paused,
    GameOver,
    Victory
}

public class GameState
{
    public int Level { get; set; } = 0;
    public int Score { get; set; } = 0;
    public int VirusesRemaining { get; set; } = 0;
    public int VirusesCleared { get; set; } = 0;
    public GameStatus Status { get; set; } = GameStatus.NotStarted;

    /// <summary>
    /// Fall interval in milliseconds - decreases with level
    /// </summary>
    public int FallSpeed => Math.Max(100, 800 - (Level * 30));

    /// <summary>
    /// Number of viruses to place based on level
    /// </summary>
    public int VirusCount => Math.Min(84, (Level + 1) * 4);

    /// <summary>
    /// Rows where viruses can spawn (leave top rows clear for pills)
    /// </summary>
    public int VirusStartRow => 4;  // Viruses start from row 4 down

    public void Reset(int level = 0)
    {
        Level = level;
        Score = 0;
        VirusesCleared = 0;
        Status = GameStatus.NotStarted;
    }

    public void StartLevel()
    {
        Status = GameStatus.Playing;
    }

    public void AddScore(int virusesCleared, int comboMultiplier = 1)
    {
        // Scoring: 100 points per virus, multiplied by combo
        Score += virusesCleared * 100 * comboMultiplier;
        VirusesCleared += virusesCleared;
    }

    public void CheckVictory(int remainingViruses)
    {
        VirusesRemaining = remainingViruses;
        if (remainingViruses == 0)
        {
            Status = GameStatus.Victory;
        }
    }

    public void SetGameOver()
    {
        Status = GameStatus.GameOver;
    }
}
