using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Header("Mezclador de música")]
    [SerializeField] private AudioMixer audioMixer;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ApplySavedVolume();
    }

    private void ApplySavedVolume()
    {
        if (audioMixer == null)
        {
            Debug.LogWarning(
                "MusicManager: falta asignar el AudioMixer."
            );
            return;
        }

        float volume = PlayerPrefs.GetFloat(
            "MusicVolumeValue",
            0.7f
        );

        ApplyMixerVolume(volume);
    }

    private void ApplyMixerVolume(float value)
    {
        audioMixer.SetFloat(
            "MusicVolume",
            Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f
        );
    }

    public void SetVolume(float volume)
    {
        if (audioMixer == null)
        {
            return;
        }

        volume = Mathf.Clamp01(volume);

        ApplyMixerVolume(volume);

        PlayerPrefs.SetFloat(
            "MusicVolumeValue",
            volume
        );

        PlayerPrefs.Save();
    }

    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }
}