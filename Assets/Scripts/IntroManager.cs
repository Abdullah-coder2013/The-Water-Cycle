using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public Transform startPos;
    [SerializeField] private Transform endPos;
    public float speed = 5f;
    public GameObject scenePrefabStart;
    public GameObject scenePrefab;
    [SerializeField] private List<GameObject> obstaclePrefab;
    [SerializeField] private GameObject startMenu;

    [Header("Information System Integration")]
    [SerializeField] private bool enableFactDisplay = true;
    [SerializeField] private float factStartDelay = 2f;

    private List<GameObject> sceneSegments = new List<GameObject>();
    public int sceneSegmentsToSpawn = 4; // Number of segments to spawn initially

    [Header("Audio")]
    [SerializeField] private AudioClip rainMusic;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource buttonClickSource;
    [SerializeField] private AudioClip buttonClickClip;

    private LevelLoader levelLoader;

    private float width = 12.5f;
    private bool initialized = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("[IntroManager] Start called - looking for LevelLoader");
        var levelLoaderGO = GameObject.Find("LevelLoader");
        if (levelLoaderGO == null)
        {
            Debug.LogError("[IntroManager] Could not find LevelLoader GameObject!");
        }
        else
        {
            levelLoader = levelLoaderGO.GetComponent<LevelLoader>();
            if (levelLoader == null)
            {
                Debug.LogError("[IntroManager] LevelLoader GameObject found but no LevelLoader component!");
            }
            else
            {
                Debug.Log("[IntroManager] LevelLoader found and assigned successfully");
            }
        }
        
        musicSource.clip = rainMusic;
        musicSource.loop = true;
        musicSource.Play();
        GetWidth();
        BuildNewScenePref(true);
        
        Information.OnFactSequenceCompleted += OnOnFactSequenceCompleted;
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

    private void OnOnFactSequenceCompleted()
    {
        Debug.Log("[IntroManager] OnFactSequenceCompleted called");
        // Handle fact sequence completion if needed
        if (levelLoader == null)
        {
            Debug.LogError("[IntroManager] levelLoader is null when trying to load Precipitation scene!");
            return;
        }
        Debug.Log("[IntroManager] Loading Precipitation scene via LevelLoader");
        levelLoader.LoadLevel("Precipitation");
    }

    private IEnumerator StartFactsWithDelay()
    {
        yield return new WaitForSeconds(factStartDelay);
        Information.Instance.StartFactSequence(WaterCycleStage.Intro);
    }

    private void GetWidth()
    {
        if (scenePrefab != null)
        {
            var renderer = scenePrefab.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                width = renderer.bounds.size.y; // Assuming the width is along the y-axis
            }
            else
            {
                Debug.LogWarning("Renderer not found on scenePrefab.");
            }
        }
        else
        {
            Debug.LogError("scenePrefab is not assigned.");
        }
    }

    public void StartGame()
    {
        initialized = true;
        startMenu.SetActive(false);
        // Start displaying precipitation facts
        if (enableFactDisplay && Information.Instance != null)
        {
            StartCoroutine(StartFactsWithDelay());
        }
    }

    private void Update()
    {

        if (!initialized) return; // Ensure the game is initialized before updating
        foreach (var segment in sceneSegments)
        {
            segment.transform.position += Vector3.up * speed * Time.deltaTime;
            if (segment.transform.position.y > endPos.position.y)
            {
                sceneSegments.Remove(segment);
                Destroy(segment);
                BuildNewScenePref();
                break; // Exit the loop to avoid modifying the collection while iterating
            }
        }
    }

    private void BuildNewScenePref(bool firstBuild = false)
    {
        if (firstBuild)
        {
            var distance = width;
            var newScene = Instantiate(scenePrefabStart, startPos.position, Quaternion.identity, transform);
            sceneSegments.Add(newScene);
            for (var i = 0; i < sceneSegmentsToSpawn; i++)
            {
                var newSegment = Instantiate(scenePrefabStart, startPos.position + new Vector3(0, -distance * (i + 1), 0), Quaternion.identity, transform);
                sceneSegments.Add(newSegment);
            }
        }
        else
        {
            var newSegment = Instantiate(scenePrefab, sceneSegments[0].transform.position + new Vector3(0, -width * sceneSegments.Count, 0), Quaternion.identity, transform);
            sceneSegments.Add(newSegment);
        }
    }
    private void OnDestroy()
    {
        // Stop fact sequence when minigame ends
        if (Information.Instance != null && Information.Instance.IsPlayingSequence())
        {
            Information.Instance.StopFactSequence();
        }
    }
    
    // Public method to manually start facts (can be called from UI or other scripts)
    public void StartPrecipitationFacts()
    {
        if (Information.Instance != null)
        {
            Information.Instance.StartFactSequence(WaterCycleStage.Intro);
        }
    }
    
    // Public method to stop facts
    public void StopFacts()
    {
        if (Information.Instance != null)
        {
            Information.Instance.StopFactSequence();
        }
    }
}
