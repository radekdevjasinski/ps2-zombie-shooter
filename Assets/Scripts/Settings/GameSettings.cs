using UnityEngine;

public static class GameSettings
{
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string MouseSensitivityKey = "MouseSensitivity";
    private const string QualityLevelKey = "QualityLevel";

    public static float LoadMusicVolume(float defaultVolume)
    {
        return PlayerPrefs.GetFloat(MusicVolumeKey, defaultVolume);
    }

    public static void SaveMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

    public static float LoadSfxVolume(float defaultVolume)
    {
        return PlayerPrefs.GetFloat(SfxVolumeKey, defaultVolume);
    }

    public static void SaveSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, volume);
    }

    public static float LoadMouseSensitivity(float defaultSensitivity)
    {
        return PlayerPrefs.GetFloat(MouseSensitivityKey, defaultSensitivity);
    }

    public static void SaveMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat(MouseSensitivityKey, sensitivity);
    }

    public static void SaveQualityLevel(int qualityLevel)
    {
        PlayerPrefs.SetInt(QualityLevelKey, qualityLevel);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplySavedQualityLevel()
    {
        if (PlayerPrefs.HasKey(QualityLevelKey))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(QualityLevelKey));
        }
    }
}
