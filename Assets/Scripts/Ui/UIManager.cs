using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("GameMode Type")]
    [SerializeField]
    TMP_Text m_currentEnemyKilledText = null;

    [SerializeField]
    TMP_Text m_maxNumberOfEnemyText = null;

    [SerializeField]
    GameMode m_initialGameMode = GameMode.DefeatAllEnemies;

    [Header("Game Conditions")]
    [SerializeField]
    float m_timeToOpen = 0.5f;

    [SerializeField]
    GameObject m_gameResultPanel = null;

    [SerializeField]
    TMP_Text m_gameResultsTxt = null;

    [Header("PauseMenu Settings")]
    [SerializeField]
    GameObject m_pauseMenu = null;

    [SerializeField]
    LoadingManager m_loadingManager = null;

    [SerializeField]
    string m_mainMenuSceneName;

    [SerializeField]
    AudioClip m_mainMenuClip = null;

    [SerializeField]
    AudioClip m_currentSceneClip = null;

    bool m_isSceneChanging = false;

    void Start()
    {
        m_gameResultPanel.SetActive(false);
        m_pauseMenu.SetActive(false);
        m_loadingManager.gameObject.SetActive(false);
        m_isSceneChanging = false;

        GameEvents.OnGameWon += HandleGameWon;
        GameEvents.OnGameLost += HandleGameLost;

        GameEvents.GameStarted(m_initialGameMode);
    }

    public void UpdateEnemyCount(int noOfEnemies, int maxNumberOfEnemies)
    {
        m_currentEnemyKilledText.text = noOfEnemies.ToString();
        m_maxNumberOfEnemyText.text = maxNumberOfEnemies.ToString();
    }

    void HandleGameWon()
    {
        if (m_isSceneChanging)
            return;
        StartCoroutine(OpenPanel(m_gameResultPanel, "You Won", Color.white));
    }

    void HandleGameLost()
    {
        if (m_isSceneChanging)
            return;
        StartCoroutine(OpenPanel(m_gameResultPanel, "You Lost", Color.red));
    }

    IEnumerator OpenPanel(GameObject gameObject, string message, Color textColor)
    {
        yield return new WaitForSeconds(m_timeToOpen);

        m_gameResultsTxt.text = message;
        m_gameResultsTxt.color = textColor;
        gameObject.SetActive(true);

        Time.timeScale = 0;
    }

    public void OpenPauseMenu()
    {
        m_pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ClosePauseMenu()
    {
        m_pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenMainMenu()
    {
        m_isSceneChanging = true;
        Time.timeScale = 1;

        m_loadingManager.gameObject.SetActive(true);
        m_loadingManager.LoadScene(m_mainMenuSceneName, m_mainMenuClip);

        m_loadingManager.StartLoading();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        m_isSceneChanging = true;
        m_loadingManager.gameObject.SetActive(true);
        string activeSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(RestartSceneAsync(activeSceneName));
    }

    IEnumerator RestartSceneAsync(string sceneName)
    {
        m_loadingManager.LoadScene(sceneName, m_currentSceneClip);
        m_loadingManager.StartLoading();

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);

        // Resetting the game state with the initial game mode
        GameEvents.GameStarted(m_initialGameMode);
        m_isSceneChanging = false;
    }

    void OnDestroy()
    {
        GameEvents.OnGameWon -= HandleGameWon;
        GameEvents.OnGameLost -= HandleGameLost;
    }
}
