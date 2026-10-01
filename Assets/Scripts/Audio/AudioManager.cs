using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const int SfxSourceCount = 8;
    private const float MinSfxPitch = 0.7f;
    private const float MaxSfxPitch = 1.3f;

    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public List<AudioClip> sfxClips;

    public float MusicVolume => musicSource.volume;
    public float SfxVolume => sfxSource.volume;

    private readonly Dictionary<string, AudioClip> sfxClipsByName = new Dictionary<string, AudioClip>();
    private AudioSource[] sfxSources;
    private int nextSfxSourceIndex;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        musicSource.ignoreListenerPause = true;
        IndexSfxClipsByName();
        CreateSfxSources();
        ApplySavedVolumes();
    }

    void Start()
    {
        PlayMusic(backgroundMusic);
    }

    private void IndexSfxClipsByName()
    {
        foreach (AudioClip clip in sfxClips)
        {
            if (clip == null)
            {
                continue;
            }

            if (!sfxClipsByName.TryAdd(clip.name, clip))
            {
                Debug.LogWarning($"Duplicate sound effect name '{clip.name}' in AudioManager; keeping the first clip.", this);
            }
        }
    }

    private void CreateSfxSources()
    {
        sfxSources = new AudioSource[SfxSourceCount];
        sfxSources[0] = sfxSource;
        for (int i = 1; i < SfxSourceCount; i++)
        {
            AudioSource source = sfxSource.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            source.spatialBlend = sfxSource.spatialBlend;
            sfxSources[i] = source;
        }
    }

    private void ApplySavedVolumes()
    {
        ApplyMusicVolume(GameSettings.LoadMusicVolume(musicSource.volume));
        ApplySfxVolume(GameSettings.LoadSfxVolume(sfxSource.volume));
    }

    public void PlayMusic(AudioClip music)
    {
        if (music == null)
        {
            return;
        }

        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(string clipName)
    {
        if (!sfxClipsByName.TryGetValue(clipName, out AudioClip clip))
        {
            Debug.LogWarning($"Sound effect '{clipName}' not found in AudioManager.", this);
            return;
        }

        AudioSource source = sfxSources[nextSfxSourceIndex];
        nextSfxSourceIndex = (nextSfxSourceIndex + 1) % sfxSources.Length;
        source.pitch = Random.Range(MinSfxPitch, MaxSfxPitch);
        source.PlayOneShot(clip);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        ApplyMusicVolume(volume);
        GameSettings.SaveMusicVolume(MusicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        ApplySfxVolume(volume);
        GameSettings.SaveSfxVolume(SfxVolume);
    }

    private void ApplyMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    private void ApplySfxVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        foreach (AudioSource source in sfxSources)
        {
            source.volume = clampedVolume;
        }
    }
}
