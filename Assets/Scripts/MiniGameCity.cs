using UnityEngine;

public class MiniGameCity : MonoBehaviour
{
    [Header("Audio")] [SerializeField] private AudioClip rainMusic;
    [SerializeField] private AudioClip rainSFX;
    [SerializeField]private AudioSource musicSource;
    [SerializeField] private AudioSource rainSFXSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource.loop = true;
        musicSource.clip = rainMusic;
        musicSource.Play();
        rainSFXSource.loop = true;
        rainSFXSource.clip = rainSFX;
        rainSFXSource.Play();
    }

}
