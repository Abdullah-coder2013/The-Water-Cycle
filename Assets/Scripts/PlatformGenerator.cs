using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public List<GameObject> platformPrefab;
    public GameObject[] backgroundPrefabs;
    public int numberOfPlatforms = 10;
    public float levelWidth = 3f;
    public float minY = .2f;
    public float maxY = 1.5f;

    private Transform playerTransform;
    private float lastPlatformY;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastPlatformY = playerTransform.position.y;
        Vector3 spawnPosition = new Vector3();

        for (int i = 0; i < numberOfPlatforms; i++)
        {
            spawnPosition.y += Random.Range(minY, maxY);
            spawnPosition.x = Random.Range(-levelWidth, levelWidth);
            GameObject platformPrefab = this.platformPrefab[Random.Range(0, this.platformPrefab.Count)];
            Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
            lastPlatformY = spawnPosition.y;
        }

    }

    void Update()
    {
        if (playerTransform.position.y + 10f > lastPlatformY)
        {
            Vector3 spawnPosition = new Vector3();
            spawnPosition.y = lastPlatformY + Random.Range(minY, maxY);
            spawnPosition.x = Random.Range(-levelWidth, levelWidth);
            GameObject platformPrefab = this.platformPrefab[Random.Range(0, this.platformPrefab.Count)];
            Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
            lastPlatformY = spawnPosition.y;
            
        }
    }


}