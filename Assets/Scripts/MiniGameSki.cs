using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MiniGameSki : MonoBehaviour
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
    
    [Header("Audio")] [SerializeField] private AudioClip rainMusic;
    [SerializeField]private AudioSource musicSource;

    private List<GameObject> sceneSegments = new List<GameObject>();
    public int sceneSegmentsToSpawn = 6; // Number of segments to spawn initially

    private float width = 12.5f;
    private bool initialized = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Information.Instance != null)
        {
            Information.OnFactSequenceCompleted += InformationOnOnFactSequenceCompleted;
        }
        GetWidth();
        BuildNewScenePref(true);
        musicSource.clip = rainMusic;
        musicSource.loop = true;
        musicSource.Play();
        // Start displaying precipitation facts
        if (enableFactDisplay && Information.Instance != null)
        {
            StartCoroutine(StartFactsWithDelay());
        }
    }

    private void InformationOnOnFactSequenceCompleted()
    {
        // Handle fact sequence completion if needed
        SceneManager.LoadScene("River");
    }

    private IEnumerator StartFactsWithDelay()
    {
        yield return new WaitForSeconds(factStartDelay);
        Information.Instance.StartFactSequence(WaterCycleStage.Drainage);
    }
    
    private void OnDestroy()
    {
        // Stop fact sequence when minigame ends
        if (Information.Instance != null && Information.Instance.IsPlayingSequence())
        {
            Information.Instance.StopFactSequence();
        }
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

    private void Update()
    {
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
    
}

