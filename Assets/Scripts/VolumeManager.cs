using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private AudioMixer musicMixer;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider narrationSlider;

    [Header("Button Click Sound")]
    [SerializeField] private AudioSource buttonClickSource;
    [SerializeField] private AudioClip buttonClickClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("Music", 0.25f);
        UpdateMusicVolume();
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 0.5f);
        UpdateSFXVolume();
        narrationSlider.value = PlayerPrefs.GetFloat("Narration", 1f);
        UpdateNarrationVolume();
    }

    public void UpdateMusicVolume()
    {
        var value = musicSlider.value;
        musicMixer.SetFloat("music", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Music", musicSlider.value);
    }
    public void UpdateSFXVolume()
    {
        var value = sfxSlider.value;
        musicMixer.SetFloat("sfx", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFX", sfxSlider.value);
    }
    public void UpdateNarrationVolume()
    {
        var value = narrationSlider.value;
        musicMixer.SetFloat("narration", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Narration", narrationSlider.value);
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickSource != null && buttonClickClip != null)
        {
            buttonClickSource.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch for variety
            buttonClickSource.PlayOneShot(buttonClickClip);
        }
        else
        {
            Debug.LogWarning("[IntroManager] Button click sound not set up properly.");
        }
    }
}
