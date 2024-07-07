using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameModeSelector : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    StageLoop m_stageLoop = null;

    [SerializeField]
    LoadingManager m_loadingMenu = null;


    [Header("Game Select Scene")]
    [SerializeField]
    string m_sceneName;

    [SerializeField]
    AudioClip m_nextSceneClip;
    

    [Header("UI Elements")]
    [SerializeField]
    TMP_Text m_maxActiveEnemiesText;

    [SerializeField]
    TMP_Text m_totalEnemiesText;

    [SerializeField]
    TMP_Text m_errorText = null;

    [SerializeField]
    float m_errorDisplayTime = .5f;

    int m_maxActiveEnemies = INITIAL_MAX_ACTIVE_ENEMIES;
    int m_totalEnemies = INITIAL_TOTAL_ENEMIES;

    const int INITIAL_MAX_ACTIVE_ENEMIES = 2;
    const int INITIAL_TOTAL_ENEMIES = 3;
    const int MIN_MAX_ACTIVE_ENEMIES = 2;
    const int MAX_MAX_ACTIVE_ENEMIES = 4;
    const int MIN_TOTAL_ENEMIES = 3;
    const int MAX_TOTAL_ENEMIES = 99;

    void Start()
    {
        UpdateUI();
        m_loadingMenu.gameObject.SetActive(false);
    }

    public void IncreaseMaxActiveEnemies()
    {
        if (m_maxActiveEnemies < MAX_MAX_ACTIVE_ENEMIES)
        {
            m_maxActiveEnemies++;
        }
        else
        {
            m_maxActiveEnemies = MIN_MAX_ACTIVE_ENEMIES;
        }
        UpdateMaxActiveEnemies();
    }

    public void DecreaseMaxActiveEnemies()
    {
        if (m_maxActiveEnemies > MIN_MAX_ACTIVE_ENEMIES)
        {
            m_maxActiveEnemies--;
        }
        else
        {
            m_maxActiveEnemies = MAX_MAX_ACTIVE_ENEMIES;
        }
        UpdateMaxActiveEnemies();
    }

    public void IncreaseTotalEnemies()
    {
        if (m_totalEnemies < MAX_TOTAL_ENEMIES)
        {
            m_totalEnemies++;
        }
        else
        {
            m_totalEnemies = MIN_TOTAL_ENEMIES;
        }
        UpdateTotalEnemies();
    }

    public void DecreaseTotalEnemies()
    {
        if (m_totalEnemies > MIN_TOTAL_ENEMIES)
        {
            m_totalEnemies--;
        }
        else
        {
            m_totalEnemies = MAX_TOTAL_ENEMIES;
        }
        UpdateTotalEnemies();
    }

    void UpdateMaxActiveEnemies()
    {
        GameModeConfiguration.MaxActiveEnemies = m_maxActiveEnemies;
        m_maxActiveEnemiesText.text = m_maxActiveEnemies.ToString();
    }

    void UpdateTotalEnemies()
    {
        GameModeConfiguration.TotalEnemies = m_totalEnemies;
        m_totalEnemiesText.text = m_totalEnemies.ToString();
    }

    void UpdateUI()
    {
        m_maxActiveEnemiesText.text = m_maxActiveEnemies.ToString();
        m_totalEnemiesText.text = m_totalEnemies.ToString();
    }

    public void ApplySettings()
    {
        if(m_maxActiveEnemies > m_totalEnemies)
        {
            StartCoroutine(DisplayErrorText("Active Enemies cannot be more than Total"));
            return;
        }
        GameModeConfiguration.MaxActiveEnemies = m_maxActiveEnemies;
        GameModeConfiguration.TotalEnemies = m_totalEnemies;

        SelectDefeatAllEnemiesMode();

        m_loadingMenu.gameObject.SetActive(true);
        m_loadingMenu.LoadScene(m_sceneName, m_nextSceneClip);

        gameObject.SetActive(false);

        m_loadingMenu.StartLoading();
    }

    public void SelectDefeatAllEnemiesMode()
    {
        m_stageLoop.SetGameMode(GameMode.DefeatAllEnemies);
    }

    IEnumerator DisplayErrorText(string message)
    {
        m_errorText.text = message;
        yield return new WaitForSeconds(m_errorDisplayTime);
        m_errorText.text = "";
    }
}
