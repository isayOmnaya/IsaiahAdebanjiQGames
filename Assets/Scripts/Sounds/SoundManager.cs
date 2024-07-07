using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField]
    Sound[] m_sounds;

    [SerializeField]
    float m_audioSourceSize = 5;

    [SerializeField]
    float m_audioSourceVolume = 1;

    [SerializeField]
    AudioClip m_backGroundSound;

    [SerializeField]
    GameObject m_soundOnObj = null;

    [SerializeField]
    GameObject m_soundOffObj = null;

    bool m_isPlaying = true;

    Dictionary<SoundTag, Sound> m_soundDictionary = new Dictionary<SoundTag, Sound>();
    List<AudioSource> m_audioSources = new List<AudioSource>();
    Dictionary<SoundTag, List<AudioSource>> m_playingSounds =
        new Dictionary<SoundTag, List<AudioSource>>();

    void Start()
    {
        foreach (Sound sound in m_sounds)
        {
            m_soundDictionary.Add(sound.m_Id, sound);
            m_playingSounds[sound.m_Id] = new List<AudioSource>();
        }

        // Create a pool of audio sources
        for (int i = 0; i < m_audioSourceSize; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = m_audioSourceVolume;
            m_audioSources.Add(audioSource);
        }
        ServiceLocator.RegisterSoundManager(this);
        ToggleBackGroundSound();
    }

    public void PlaySound(SoundTag tag)
    {
        if (!m_soundDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Sound ID not found: " + tag);
            return;
        }

        Sound sound = m_soundDictionary[tag];

        if (sound.m_CurrentInstances >= sound.m_MaxInstances)
        {
            return; // Maximum instances reached, do not play the sound
        }

        AudioSource availableSource = GetAvailableAudioSource();
        if (availableSource == null)
        {
            // Debug.LogWarning("No available audio source.");
            return;
        }

        AudioClip clip = sound.m_Clips[Random.Range(0, sound.m_Clips.Length)];
        availableSource.clip = clip;
        availableSource.Play();

        sound.m_CurrentInstances++;
        m_playingSounds[tag].Add(availableSource);
        StartCoroutine(ResetSoundInstanceCount(sound, clip.length, availableSource));
    }

    AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in m_audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return null; // No available audio source
    }

    private IEnumerator ResetSoundInstanceCount(Sound sound, float delay, AudioSource source)
    {
        yield return new WaitForSeconds(delay);
        sound.m_CurrentInstances--;
        m_playingSounds[sound.m_Id].Remove(source);
    }

    public void ToggleBackGroundSound()
    {
        m_isPlaying = !m_isPlaying;
        m_soundOnObj.SetActive(!m_isPlaying);
        m_soundOffObj.SetActive(m_isPlaying);

        if (m_isPlaying)
        {
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.StopMusic();
            }
        }
        else
        {
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlayMusic(m_backGroundSound);
            }
        }
    }

    public void StopSound(SoundTag tag)
    {
        if (!m_playingSounds.ContainsKey(tag) || m_playingSounds[tag].Count == 0)
        {
            // Debug.LogWarning("No sound playing with tag: " + tag);
            return;
        }

        List<AudioSource> sourcesToStop = new List<AudioSource>(m_playingSounds[tag]);
        foreach (AudioSource source in sourcesToStop)
        {
            source.Stop();
            m_playingSounds[tag].Remove(source);
        }

        m_soundDictionary[tag].m_CurrentInstances = 0;
    }
}

[System.Serializable]
public class Sound
{
    public SoundTag m_Id;
    public AudioClip[] m_Clips;
    public int m_MaxInstances;

    [HideInInspector]
    public int m_CurrentInstances;
}

public enum SoundTag
{
    //BackgroundMusic,
    Explosion = 0,
    bullet,
    WaterExplosion,
    Aim,
    FireYell,
    RumThrow,
    PickUp,
    ShipCollision
}
