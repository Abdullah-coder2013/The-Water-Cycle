using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MiniGame1Manager : MonoBehaviour
{
    public Transform startPos;
    [SerializeField] private Transform endPos;
    public float speed = 5f;
    public GameObject scenePrefab;
    [SerializeField] private List<GameObject> obstaclePrefab;
    
    [Header("Information System Integration")]
    [SerializeField] private bool enableFactDisplay = true;
    [SerializeField] private float factStartDelay = 2f;
    
    private List<GameObject> sceneSegments = new List<GameObject>();

    private float width = 12.5f;
    private bool initialized = false;

    public int sceneSegmentsToSpawn = 4;
    
    [Header("Audio")] [SerializeField] private AudioClip rainMusic;
    [SerializeField] private AudioClip snowMusic;
    [SerializeField] private AudioClip rainSFX;
    [SerializeField]private AudioSource musicSource;
    [SerializeField] private AudioSource rainSFXSource;

    public bool snow = false;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        musicSource.loop = true;
        if (snow)
        {
            musicSource.clip = snowMusic;
        }
        else
        {
            musicSource.clip = rainMusic;
            rainSFXSource.loop = true;
            rainSFXSource.clip = rainSFX;
            rainSFXSource.Play();
        }
        musicSource.Play();
        
        GetWidth();
        BuildNewScenePref(true);
        StartCoroutine(SpeedIncrease());
        initialized = true;
        
        // Start displaying precipitation facts
        if (enableFactDisplay && Information.Instance != null)
        {
            StartCoroutine(StartFactsWithDelay());
        }
    }
    
    private IEnumerator StartFactsWithDelay()
    {
        yield return new WaitForSeconds(factStartDelay);
        Information.Instance.StartFactSequence(WaterCycleStage.Precipitation);
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

    private IEnumerator SpeedIncrease()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // Wait for 10 seconds before increasing speed
            if (speed >= 20f) // Cap the speed at 20
            {
                speed = 20f;
                yield break; // Exit the coroutine if speed reaches the cap
            }
            speed += 2.5f; // Increase speed by 1 unit
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

    private void BuildNewScenePref(bool firstBuild=false)
    {
        if (firstBuild)
        {
            var distance = width;
            var newScene = Instantiate(scenePrefab, startPos.position, Quaternion.identity, transform);
            sceneSegments.Add(newScene);
            for (var i=0; i< sceneSegmentsToSpawn; i++)
            {
                var newSegment = Instantiate(scenePrefab, startPos.position + new Vector3(0, -distance * (i + 1),0), Quaternion.identity, transform);
                sceneSegments.Add(newSegment);
            }
        }
        else
        {
            var newSegment = Instantiate(scenePrefab, sceneSegments[0].transform.position + new Vector3(0, -width * sceneSegments.Count,0), Quaternion.identity, transform);
            sceneSegments.Add(newSegment);
            GenerateObstaclesForSceneSegment(newSegment);
        }
    }
    
    private void GenerateObstaclesForSceneSegment(GameObject segment)
    {
        int obstacleCount = Random.Range(1, 6); // Random number of obstacles between 1 and 3
        for (int i = 0; i < obstacleCount; i++)
        {
            // Instantiate obstacles at random positions within the segment
            Vector3 position = segment.transform.position + new Vector3(Random.Range(-10, 10), Random.Range(-2f, 2f), 0);
            var randomObstaclePrefab = obstaclePrefab[Random.Range(0, obstaclePrefab.Count)];
            var obstacle = Instantiate(randomObstaclePrefab, position, Quaternion.identity, segment.transform);
            obstacle.transform.localScale = new Vector3(0.25f, 0.25f, 0); // Set the scale of the obstacle
            // You can customize the obstacle here if needed
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
            Information.Instance.StartFactSequence(WaterCycleStage.Precipitation);
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
