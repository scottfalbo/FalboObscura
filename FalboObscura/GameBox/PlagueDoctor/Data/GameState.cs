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

public enum GameType
{
    TypeA,  // Progressive: levels increase viruses and speed
    TypeB   // Single round: user selects speed and virus count
}

public enum SpeedSetting
{
    Low = 0,
    Medium = 1,
    High = 2
}

public class GameState
{
    public GameType Type { get; set; } = GameType.TypeA;
    public int Level { get; set; } = 0;
    public int Score { get; set; } = 0;
    public int VirusesRemaining { get; set; } = 0;
    public int VirusesCleared { get; set; } = 0;
    public GameStatus Status { get; set; } = GameStatus.NotStarted;
    
    // Type B settings
    public SpeedSetting SelectedSpeed { get; set; } = SpeedSetting.Medium;
    public int SelectedVirusLevel { get; set; } = 5;  // 0-20 range

    /// <summary>
    /// Fall interval in milliseconds - decreases with level or based on selected speed
    /// </summary>
    public int FallSpeed => Type == GameType.TypeA 
        ? Math.Max(100, 800 - (Level * 30))
        : SelectedSpeed switch
        {
            SpeedSetting.Low => 700,
            SpeedSetting.Medium => 450,
            SpeedSetting.High => 200,
            _ => 450
        };

    /// <summary>
    /// Number of viruses to place based on level or selected virus level
    /// </summary>
    public int VirusCount => Type == GameType.TypeA
        ? Math.Min(84, (Level + 1) * 4)
        : Math.Min(84, (SelectedVirusLevel + 1) * 4);

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
