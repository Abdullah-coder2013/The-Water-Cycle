using System.Collections;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    [Header("Information System Integration")]
    [SerializeField] private bool enableFactDisplay = true;
    [SerializeField] private float factStartDelay = 2f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip oceanMusic;
    [SerializeField] private AudioSource musicSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource.clip = oceanMusic;
        musicSource.loop = true;
        musicSource.Play();
        if (enableFactDisplay && Information.Instance != null)
        {
            StartCoroutine(StartFactsWithDelay());
        }
    }
    private IEnumerator StartFactsWithDelay()
    {
        yield return new WaitForSeconds(factStartDelay);
        Information.Instance.StartFactSequence(WaterCycleStage.Ocean);
    }
    
    private void OnDestroy()
    {
        // Stop fact sequence when minigame ends
        if (Information.Instance != null && Information.Instance.IsPlayingSequence())
        {
            Information.Instance.StopFactSequence();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
