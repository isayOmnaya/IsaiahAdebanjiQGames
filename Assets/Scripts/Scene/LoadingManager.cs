using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    Slider m_loadingBar;

    [SerializeField]
    TMP_Text m_loadingText;

    private static string m_nextSceneName;
    private static AudioClip m_nextSceneMusic;

    public void LoadScene(string sceneName, AudioClip music)
    {
        m_nextSceneName = sceneName;
        m_nextSceneMusic = music;
    }

    public void StartLoading()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    private IEnumerator LoadAsyncOperation()
    {
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(m_nextSceneName);

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic(m_nextSceneMusic, 1.0f);
        }

        while (!gameLevel.isDone)
        {
            float progress = Mathf.Clamp01(gameLevel.progress / 0.9f);
            m_loadingBar.value = progress;
            m_loadingText.text = (progress * 100f).ToString("F0") + "%";

            yield return null;
        }
    }
}
