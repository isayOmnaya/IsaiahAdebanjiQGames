using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField]
    AudioSource m_musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1.0f)
    {
        StartCoroutine(FadeMusic(clip, fadeDuration));
    }

    private IEnumerator FadeMusic(AudioClip newClip, float fadeDuration)
    {
        if (m_musicSource.isPlaying)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                m_musicSource.volume = 1 - (t / fadeDuration);
                yield return null;
            }
            m_musicSource.Stop();
        }

        m_musicSource.clip = newClip;
        m_musicSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            m_musicSource.volume = t / fadeDuration;
            yield return null;
        }
        m_musicSource.volume = 1;
    }

    public void StopMusic()
    {
        if (m_musicSource != null)
        {
            m_musicSource.Stop();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (m_musicSource != null)
        {
            m_musicSource.clip = clip;
            m_musicSource.Play();
        }
    }
}
