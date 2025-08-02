using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator1 : MonoBehaviour
{
    public GameObject scenePrefab; // Prefab for the scene segments
    private List<GameObject> sceneSegments = new List<GameObject>();
    public int sceneSegmentsToSpawn = 6; // Number of segments to spawn initially

    public GameObject player;

    private float width = 12.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetWidth();
        BuildNewScenePref(true);
        player = GameObject.FindGameObjectWithTag("Player");
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

    // Update is called once per frame
    private void Update()
    {
        if (player.transform.position.y + 10f > sceneSegments[sceneSegments.Count - 1].transform.position.y)
        {
            BuildNewScenePref();
        }
    }
    private void BuildNewScenePref(bool firstBuild = false)
    {
        if (firstBuild)
        {
            var distance = width;
            var newScene = Instantiate(scenePrefab, new Vector3(0,0,0), Quaternion.identity, transform);
            sceneSegments.Add(newScene);
            for (var i = 0; i < sceneSegmentsToSpawn; i++)
            {
                var newSegment = Instantiate(scenePrefab, new Vector3(0,0,0) + new Vector3(0, distance * (i + 1), 0), Quaternion.identity, transform);
                sceneSegments.Add(newSegment);
            }
        }
        else
        {
            var newSegment = Instantiate(scenePrefab, sceneSegments[0].transform.position + new Vector3(0, width * sceneSegments.Count, 0), Quaternion.identity, transform);
            sceneSegments.Add(newSegment);
        
        }
    }
}
