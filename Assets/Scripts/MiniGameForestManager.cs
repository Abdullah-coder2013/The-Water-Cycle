using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MiniGameForestManager : MonoBehaviour
{
    private PlayerForestParkour playerForestParkour;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private Transform startPos;
    [Header("Information System Integration")]
    [SerializeField] private bool enableFactDisplay = true;
    [SerializeField] private float factStartDelay = 2f;
    [Header("Audio")] [SerializeField] private AudioClip caveMusic;
    [SerializeField] private AudioClip caveSFX;
    [SerializeField]private AudioSource musicSource;
    [SerializeField] private AudioSource caveSFXSource;

    [Header("PlayerAudio")]
    
    [SerializeField] private AudioClip jumpSound;

    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip fallSound;

    
    private void Start()
    {
        playerForestParkour = GameObject.Find("HailDroplet").GetComponent<PlayerForestParkour>();
        musicSource.loop = true;
        musicSource.clip = caveMusic;
        musicSource.Play();
        caveSFXSource.loop = true;
        caveSFXSource.clip = caveSFX;
        caveSFXSource.Play();
        if (enableFactDisplay && Information.Instance != null)
        {
            StartCoroutine(StartFactsWithDelay());
        }
    }
    public void TurnPlayerintoWater()
    {
        var pos = playerForestParkour.transform.position;
        Destroy(playerForestParkour.gameObject);
        var water = Instantiate(waterPrefab, pos, Quaternion.identity);
        water.AddComponent<PlayerForestParkour>();
        water.GetComponent<PlayerForestParkour>().onStart = false;
        water.GetComponent<Rigidbody2D>().gravityScale = 1f;
        water.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        water.GetComponent<Rigidbody2D>().freezeRotation = true;
        Destroy(water.GetComponent<Player>());
        mainCamera.Follow = water.transform;
        water.GetComponent<PlayerForestParkour>().startPos = startPos;
        water.GetComponent<PlayerForestParkour>().speed = 4.5f; // Adjust speed as needed
        water.GetComponent<PlayerForestParkour>().hurtSound = hurtSound;
        water.GetComponent<PlayerForestParkour>().jumpSound = jumpSound;
        water.GetComponent<PlayerForestParkour>().moveSound = moveSound;
        water.GetComponent<PlayerForestParkour>().fallSound = fallSound;
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
    
}
