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

    private void Start()
    {
        miniGame1Manager = GameObject.Find("PrecipitationGameManager").GetComponent<MiniGame1Manager>();
        Information.OnFactSequenceCompleted += OnOnFactSequenceCompleted;
    }
    
    private void OnOnFactSequenceCompleted()
    {
        if (precipitationType == PrecipitationType.Rain)
        {
            SceneManager.LoadScene("CitySewage");
        }
        else if (precipitationType == PrecipitationType.Snow)
        {
            SceneManager.LoadScene("GlacierSkiing");
        }
        else if (precipitationType == PrecipitationType.Hail)
        {
            SceneManager.LoadScene("DrainageForest");
        }
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


    public enum PrecipitationType
    {
        Rain,
        Snow,
        Hail
    }
}
