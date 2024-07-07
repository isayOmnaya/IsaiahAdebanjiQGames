using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    None,
    DefeatAllEnemies
}
public class StageLoop : MonoBehaviour
{
	GameMode m_currentGameMode = GameMode.None;
	IGameMode m_gameModeInstance;
	bool m_endGame = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
		ServiceLocator.OnEnemyManagerRegistered += InitializeGameMode;
		GameEvents.OnGameStarted += OnGameStarted;
    }

    private void Update()
    {
        switch (m_currentGameMode)
        {
            case GameMode.DefeatAllEnemies:
                if (m_gameModeInstance != null)
                    UpdateDefeatAllEnemies();
                break;
        }
    }

    private void UpdateDefeatAllEnemies()
    {
        if (CheckWinCondition(GameMode.DefeatAllEnemies))
        {
            EndGame();
        }
    }

	private void InitializeGameMode()
    {
        switch (m_currentGameMode)
        {
            case GameMode.DefeatAllEnemies:
                m_gameModeInstance = new DefeatAllEnemies();
                ((DefeatAllEnemies)m_gameModeInstance).Initialize();
                break;
        }
    }

    private bool CheckWinCondition(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.DefeatAllEnemies:
                return ServiceLocator.GetEnemyManager().AreAllEnemiesDefeated();
            default:
                return false;
        }
    }

    private void EndGame()
    {
        if(!m_endGame)
		{
			GameEvents.GameWon();
			m_endGame = true;
		}
    }

    public void SetGameMode(GameMode newGameMode)
    {
        m_currentGameMode = newGameMode;
        Debug.Log($"Game mode set to: {newGameMode}");
    }

    private void OnGameStarted(GameMode gameMode)
    {
        ResetGameState();
        m_currentGameMode = gameMode;
        InitializeGameMode();
    }

    private void ResetGameState()
    {
        m_currentGameMode = GameMode.None;
        m_gameModeInstance = null;
        m_endGame = false;
        Debug.Log("Game state has been reset.");
    }

    private void OnDestroy()
    {
        ServiceLocator.OnEnemyManagerRegistered -= InitializeGameMode;
        GameEvents.OnGameStarted -= OnGameStarted;
    }
}
