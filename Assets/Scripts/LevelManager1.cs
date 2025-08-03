using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager1 : MonoBehaviour
{
    [SerializeField] PrecipitationType precipitationType;
    [SerializeField] private GameObject rainPlayer;
    [SerializeField] private GameObject snowPlayer;
    [SerializeField] private GameObject hailPlayer;
    [SerializeField] private GameObject rainBackground;
    [SerializeField] private GameObject snowBackground;
    [SerializeField] private GameObject hailBackground;
    private MiniGame1Manager miniGame1Manager;

    [SerializeField] private Canvas canvas;
    private LevelLoader levelLoader;

    private void Start()
    {
        Debug.Log("[LevelManager1] Start called - looking for LevelLoader");
        var levelLoaderGO = GameObject.Find("LevelLoader");
        if (levelLoaderGO == null)
        {
            Debug.LogError("[LevelManager1] Could not find LevelLoader GameObject!");
        }
        else
        {
            levelLoader = levelLoaderGO.GetComponent<LevelLoader>();
            if (levelLoader == null)
            {
                Debug.LogError("[LevelManager1] LevelLoader GameObject found but no LevelLoader component!");
            }
            else
            {
                Debug.Log("[LevelManager1] LevelLoader found and assigned successfully");
            }
        }
        
        miniGame1Manager = GameObject.Find("PrecipitationGameManager").GetComponent<MiniGame1Manager>();
        Information.OnFactSequenceCompleted += OnOnFactSequenceCompleted;
    }
    
    private void OnOnFactSequenceCompleted()
    {
        Debug.Log($"[LevelManager1] OnFactSequenceCompleted triggered. Current scene: {SceneManager.GetActiveScene().name}, PrecipitationType: {precipitationType}");
        
        if (levelLoader == null)
        {
            Debug.LogError("[LevelManager1] levelLoader is null when trying to load scene!");
            return;
        }
        
        if (precipitationType == PrecipitationType.Rain)
        {
            Debug.Log("[LevelManager1] Loading CitySewage scene");
            levelLoader.LoadLevel("CitySewage");
        }
        else if (precipitationType == PrecipitationType.Snow)
        {
            Debug.Log("[LevelManager1] Loading GlacierSkiing scene");
            levelLoader.LoadLevel("GlacierSkiing");
        }
        else if (precipitationType == PrecipitationType.Hail)
        {
            Debug.Log("[LevelManager1] Loading DrainageForest scene");
            levelLoader.LoadLevel("DrainageForest");
        }
    }

    private void OnDestroy()
    {
        Debug.Log("[LevelManager1] OnDestroy called - unsubscribing from events");
        Information.OnFactSequenceCompleted -= OnOnFactSequenceCompleted;
    }

    public void Rain()
    {
        precipitationType = PrecipitationType.Rain;
        SetUpMiniGame();
    }
    public void Snow()
    {
        precipitationType = PrecipitationType.Snow;
        SetUpMiniGame();
    }
    public void Hail()
    {
        precipitationType = PrecipitationType.Hail;
        SetUpMiniGame();
    }

    private void SetUpMiniGame()
    {
        if (precipitationType == PrecipitationType.Rain)
        {
            miniGame1Manager.scenePrefab = rainBackground;
            Instantiate(rainPlayer, miniGame1Manager.startPos.position, Quaternion.identity);
        }
        else if (precipitationType == PrecipitationType.Snow)
        {
            miniGame1Manager.scenePrefab = snowBackground;
            miniGame1Manager.snow = true;
            Instantiate(snowPlayer, miniGame1Manager.startPos.position, Quaternion.identity);
        }
        else if (precipitationType == PrecipitationType.Hail)
        {
            miniGame1Manager.scenePrefab = hailBackground;
            Instantiate(hailPlayer, miniGame1Manager.startPos.position, Quaternion.identity);
        }
        miniGame1Manager.Init();
    }

    private void Update()
    {
        if (levelLoader != null && levelLoader.IsTransitioning())
        {
            canvas.enabled = false; // Disable the canvas during the transition
        }
        else
        {
            canvas.enabled = true; // Enable the canvas when not transitioning
        }
    }


    public enum PrecipitationType
    {
        Rain,
        Snow,
        Hail
    }
}
