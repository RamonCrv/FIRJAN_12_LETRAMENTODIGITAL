using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning("AudioManager não encontrado na cena!");
            }
            return instance;
        }
    }

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusicClip;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip nfcReadSound;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip gameEndSound;

    [Header("Volume Settings")]
    [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Múltiplas instâncias de AudioManager detectadas. Destruindo duplicata.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
    }

    private void InitializeAudioSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        UpdateVolumes();
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }
        else
        {
            Debug.LogWarning("AudioClip não atribuído!");
        }
    }

    private void PlayClipWithVolume(AudioClip clip, float volumeScale)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volumeScale * sfxVolume * masterVolume);
        }
        else
        {
            Debug.LogWarning("AudioClip não atribuído!");
        }
    }

    public void PlayButtonClickSound()
    {
        PlayClip(buttonClickSound);
    }

    public void PlayNFCReadSound()
    {
        PlayClip(nfcReadSound);
    }

    public void PlayGameEndSound()
    {
        PlayClip(gameEndSound);
    }

    public void PlayCorrectSound()
    {
        PlayClip(correctSound);
    }

    public void PlayWrongSound()
    {
        PlayClip(wrongSound);
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicClip != null)
        {
            musicSource.clip = backgroundMusicClip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Nenhuma música de fundo configurada!");
        }
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }

    public void MuteAll(bool mute)
    {
        if (musicSource != null) musicSource.mute = mute;
        if (sfxSource != null) sfxSource.mute = mute;
    }

    public void MuteMusic(bool mute)
    {
        if (musicSource != null)
        {
            musicSource.mute = mute;
        }
    }

    public void MuteSFX(bool mute)
    {
        if (sfxSource != null)
        {
            sfxSource.mute = mute;
        }
    }
}
