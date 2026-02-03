// ------------------------------------
// Falbo Obscura - Plague Doctor
// ------------------------------------

using GameBox.PlagueDoctor.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace GameBox.PlagueDoctor;

public partial class PlagueDoctor : ComponentBase, IDisposable
{
    [Inject] private PlagueService PlagueService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private ElementReference gameContainer;
    private System.Timers.Timer? gameTimer;

    // Game options
    private GameType selectedGameType = GameType.TypeA;
    private SpeedSetting selectedSpeed = SpeedSetting.Medium;
    private int selectedVirusLevel = 5;

    protected override void OnInitialized()
    {
        PlagueService.OnStateChanged += StateHasChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FocusGameContainer();
        }
    }

    private async Task FocusGameContainer()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("eval", "document.querySelector('.bottle-container')?.focus()");
        }
        catch
        {
            // Ignore focus errors
        }
    }

    private void StartGame()
    {
        if (selectedGameType == GameType.TypeA)
        {
            PlagueService.StartNewGame(0);
        }
        else
        {
            PlagueService.StartTypeBGame(selectedSpeed, selectedVirusLevel);
        }
        StartGameLoop();
        _ = FocusGameContainer();
    }

    private void NextLevel()
    {
        PlagueService.StartNextLevel();
        StartGameLoop();
        _ = FocusGameContainer();
    }

    private void PauseGame()
    {
        PlagueService.Pause();
        StopGameLoop();
    }

    private void ResumeGame()
    {
        PlagueService.Resume();
        StartGameLoop();
        _ = FocusGameContainer();
    }

    private void StartGameLoop()
    {
        StopGameLoop();
        gameTimer = new System.Timers.Timer(PlagueService.State.FallSpeed);
        gameTimer.Elapsed += (s, e) => InvokeAsync(() =>
        {
            PlagueService.Tick();
            StateHasChanged();

            // Use faster tick during dropping phase for smooth animation
            int targetSpeed = PlagueService.Phase == GamePhase.Dropping 
                ? 50  // Fast gravity animation
                : PlagueService.State.FallSpeed;

            if (gameTimer != null && gameTimer.Interval != targetSpeed)
            {
                gameTimer.Interval = targetSpeed;
            }

            // Stop if game ended
            if (PlagueService.State.Status != GameStatus.Playing)
            {
                StopGameLoop();
            }
        });
        gameTimer.Start();
    }

    private void StopGameLoop()
    {
        gameTimer?.Stop();
        gameTimer?.Dispose();
        gameTimer = null;
    }

    private void HandleKeyDown(KeyboardEventArgs e)
    {
        // Handle pause/resume regardless of game phase
        if (e.Key == "p" || e.Key == "P" || e.Key == "Escape")
        {
            if (PlagueService.State.Status == GameStatus.Playing)
            {
                PauseGame();
                return;
            }
            else if (PlagueService.State.Status == GameStatus.Paused)
            {
                ResumeGame();
                return;
            }
        }

        if (PlagueService.State.Status != GameStatus.Playing) return;
        if (PlagueService.Phase != GamePhase.Playing) return;  // Block input during clear/drop

        switch (e.Key)
        {
            case "ArrowLeft":
                PlagueService.MoveLeft();
                break;
            case "ArrowRight":
                PlagueService.MoveRight();
                break;
            case "ArrowDown":
                PlagueService.MoveDown();
                break;
            case "ArrowUp":
            case "x":
            case "X":
                PlagueService.RotateClockwise();
                break;
            case "z":
            case "Z":
                PlagueService.RotateCounterClockwise();
                break;
            case " ":
                PlagueService.HardDrop();
                break;
        }
    }

    private bool IsCurrentPillPosition(int row, int col)
    {
        if (PlagueService.CurrentPill == null) return false;

        var pill = PlagueService.CurrentPill;
        var (x2, y2) = pill.SecondHalfPosition;

        return (pill.X == row && pill.Y == col) || (x2 == row && y2 == col);
    }

    private string GetCellClasses(CellContent cell, int row, int col, bool isCurrentPill)
    {
        var classes = new List<string>();

        if (isCurrentPill && PlagueService.CurrentPill != null)
        {
            var pill = PlagueService.CurrentPill;
            var (x2, y2) = pill.SecondHalfPosition;

            classes.Add("pill-half");

            if (pill.X == row && pill.Y == col)
            {
                // Anchor position - use AnchorColor
                classes.Add(GetColorClass(pill.AnchorColor));
                if (pill.IsHorizontal)
                    classes.Add("connected-right");
                else
                    classes.Add("connected-down");
            }
            else
            {
                // Second position - use SecondColor
                classes.Add(GetColorClass(pill.SecondColor));
                if (pill.IsHorizontal)
                    classes.Add("connected-left");
                else
                    classes.Add("connected-up");
            }
        }
        else if (!cell.IsEmpty)
        {
            if (cell.IsVirus)
            {
                classes.Add("virus");
            }
            else
            {
                classes.Add("pill-half");
                if (cell.IsConnectedLeft) classes.Add("connected-left");
                if (cell.IsConnectedRight) classes.Add("connected-right");
                if (cell.IsConnectedUp) classes.Add("connected-up");
                if (cell.IsConnectedDown) classes.Add("connected-down");
            }

            if (cell.Color.HasValue)
            {
                classes.Add(GetColorClass(cell.Color.Value));
            }
        }

        return string.Join(" ", classes);
    }

    private string GetColorClass(PillColor color) => color switch
    {
        PillColor.Orange => "color-orange",
        PillColor.Lavender => "color-lavender",
        PillColor.Mint => "color-mint",
        _ => ""
    };

    private string GetVirusImage(PillColor color) => color switch
    {
        PillColor.Orange => "_content/GameBox/images/plague_doctor/orange_virus.png",
        PillColor.Lavender => "_content/GameBox/images/plague_doctor/purple_virus.png",
        PillColor.Mint => "_content/GameBox/images/plague_doctor/green_virus.png",
        _ => ""
    };

    public void Dispose()
    {
        StopGameLoop();
        PlagueService.OnStateChanged -= StateHasChanged;
    }
}
