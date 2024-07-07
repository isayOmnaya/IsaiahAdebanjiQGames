using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleLoop : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField]
    float m_changeInterval = 5f;

    int m_currentIndex = 0;
    bool m_start = false;

    [Header("Components")]
    [SerializeField]
    Sprite[] m_backgrounds;

    [SerializeField]
    Image m_backgroundImage;

    [SerializeField]
    MenuOpen m_startMenu = null;

    [SerializeField]
    TMP_Text m_pressStartTxt = null;

    [SerializeField]
    MenuOpen m_gameModeMenu = null;

    [SerializeField]
    MenuOpen m_defeatAllEnemyModePanel = null;

    [SerializeField]
    MenuOpen m_controlsPanel = null;

    void Start()
    {
        InvokeRepeating("ChangeBackground", 0f, m_changeInterval);
    }

    void ChangeBackground()
    {
        if (m_backgroundImage != null)
        {
            m_backgroundImage.sprite = m_backgrounds[m_currentIndex];

            m_currentIndex = (m_currentIndex + 1) % m_backgrounds.Length;
        }
    }

    void Update()
    {
        if (!m_start && Input.GetKey(KeyCode.Space))
        {
            m_startMenu.ToggleMenu();
            m_start = true;
            m_pressStartTxt.enabled = false;
        }
    }

    public void OpenGameMode()
    {
        m_gameModeMenu.ToggleMenu();
        m_startMenu.ToggleMenu();
    }

    public void OpenEnemyModePanel()
    {
        m_gameModeMenu.ToggleMenu();
        m_defeatAllEnemyModePanel.ToggleMenu();
    }

    public void OpenControlPanel()
    {
        m_startMenu.ToggleMenu();
        m_controlsPanel.ToggleMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }

    void OnDestroy()
    {
        CancelInvoke("ChangeBackground");
    }
}
