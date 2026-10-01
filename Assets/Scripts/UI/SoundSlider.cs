using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SoundSlider : MonoBehaviour
{
    public SoundType soundType;
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = soundType == SoundType.Music
            ? AudioManager.Instance.MusicVolume
            : AudioManager.Instance.SfxVolume;
    }

    public void OnSliderValueChanged()
    {
        if (soundType == SoundType.Music)
        {
            AudioManager.Instance.SetMusicVolume(slider.value);
        }
        else
        {
            AudioManager.Instance.SetSFXVolume(slider.value);
        }
    }
}
