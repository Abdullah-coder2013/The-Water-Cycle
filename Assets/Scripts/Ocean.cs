using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ocean : MonoBehaviour
{
    [Header("Information System Integration")]
    [SerializeField] private bool enableFactDisplay = true;
    [SerializeField] private float factStartDelay = 2f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip oceanMusic;
    [SerializeField] private AudioSource musicSource;

    private LevelLoader levelLoader;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        musicSource.clip = oceanMusic;
        musicSource.loop = true;
        musicSource.Play();
        if (enableFactDisplay && Information.Instance != null)
        {
            StartCoroutine(StartFactsWithDelay());
        }
        Information.OnFactSequenceCompleted += OnOnFactSequenceCompleted;
    }
    private void OnOnFactSequenceCompleted()
    {
        // Handle fact sequence completion if needed
        levelLoader.LoadLevel("Evaporation");
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
