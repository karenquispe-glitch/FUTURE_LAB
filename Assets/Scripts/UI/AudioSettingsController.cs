using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private void Start()
    {
        float musicValue = PlayerPrefs.GetFloat("MusicVolumeValue", 0.7f);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolumeValue", 0.7f);

        musicSlider.SetValueWithoutNotify(musicValue);
        sfxSlider.SetValueWithoutNotify(sfxValue);

        SetMusicVolume(musicValue);
        SetSFXVolume(sfxValue);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        if (fullscreenToggle != null)
        {
            bool fullscreen = PlayerPrefs.GetInt(
                "FullscreenValue",
                Screen.fullScreen ? 1 : 0
            ) == 1;

            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(
            "MusicVolume",
            Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f
        );

        PlayerPrefs.SetFloat("MusicVolumeValue", value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(
            "SFXVolume",
            Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f
        );

        PlayerPrefs.SetFloat("SFXVolumeValue", value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenValue", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}