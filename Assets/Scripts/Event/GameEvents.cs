using System;

public static class GameEvents
{
    public static event Action OnGameWon;
    public static event Action OnGameLost;
    public static event Action<GameMode> OnGameStarted;

    public static void GameWon()
    {
        OnGameWon?.Invoke();
    }

    public static void GameLost()
    {
        OnGameLost?.Invoke();
    }

    public static void GameStarted(GameMode gameMode)
    {
        OnGameStarted?.Invoke(gameMode);
    }
}
