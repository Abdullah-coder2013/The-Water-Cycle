using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    public GameObject[] backgroundPrefabs;
    public int numberOfBackgrounds = 5;
    public float levelWidth = 5f;
    public float ySpacing = 5f;

    private Transform playerTransform;
    private float lastBackgroundY;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastBackgroundY = playerTransform.position.y;
        Vector3 spawnPosition = new Vector3();

        for (int i = 0; i < numberOfBackgrounds; i++)
        {
            spawnPosition.y += ySpacing;
            spawnPosition.x = Random.Range(-levelWidth, levelWidth);
            GameObject background = Instantiate(backgroundPrefabs[Random.Range(0, backgroundPrefabs.Length)], spawnPosition, Quaternion.identity);
            background.transform.SetParent(transform);
            lastBackgroundY = spawnPosition.y;
        }
    }

    void Update()
    {
        if (playerTransform.position.y + 15f > lastBackgroundY)
        {
            Vector3 spawnPosition = new Vector3();
            spawnPosition.y = lastBackgroundY + ySpacing;
            spawnPosition.x = Random.Range(-levelWidth, levelWidth);
            GameObject background = Instantiate(backgroundPrefabs[Random.Range(0, backgroundPrefabs.Length)], spawnPosition, Quaternion.identity);
            background.transform.SetParent(transform);
            lastBackgroundY = spawnPosition.y;
        }
    }
}