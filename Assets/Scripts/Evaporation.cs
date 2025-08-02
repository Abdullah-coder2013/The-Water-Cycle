using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SceneManagement;

public class Evaporation : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float moveInput;
    private InputAction moveAction;
    [Header("Information System Integration")]
    [SerializeField] private bool enableFactDisplay = true;
    [SerializeField] private float factStartDelay = 2f;


    [Header("Camera Settings")]
    public Transform cameraTransform;

    [Header("Scoring")]
    public TextMeshProUGUI scoreText;
    private float topScore = 0.0f;
    
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioSource musicSource;
    
    


    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Start()
    {
        // Initialize audio
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
        rb = GetComponent<Rigidbody2D>();
        Information.OnFactSequenceCompleted += InformationOnOnFactSequenceCompleted;
        // Start displaying precipitation facts
        if (enableFactDisplay && Information.Instance != null)
        {
            StartCoroutine(StartFactsWithDelay());
        }
    }
    
    private void InformationOnOnFactSequenceCompleted()
    {
        SceneManager.LoadScene("Condensation");
    }
    private IEnumerator StartFactsWithDelay()
    {
        yield return new WaitForSeconds(factStartDelay);
        Information.Instance.StartFactSequence(WaterCycleStage.Evaporation);
    }
    private void OnDestroy()
    {
        // Stop fact sequence when minigame ends
        if (Information.Instance != null && Information.Instance.IsPlayingSequence())
        {
            Information.Instance.StopFactSequence();
        }
        // Clear last platform data
        PlayerPrefs.DeleteKey("LastPlatformY");
        PlayerPrefs.DeleteKey("LastPlatformX");
    
        PlayerPrefs.Save();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().x;

        if (rb.linearVelocity.y > 0 && transform.position.y > topScore)
        {
            topScore = transform.position.y;
        }

        scoreText.text = "Score: " + Mathf.Round(topScore).ToString();



        if (PlayerPrefs.GetFloat("LastPlatformY")-transform.position.y > 25f)
        {
            // Reset player position if they fall below the camera view
            var lastPlatformY = PlayerPrefs.GetFloat("LastPlatformY", 0f);
            var lastPlatformX = PlayerPrefs.GetFloat("LastPlatformX", 0f);
            if (lastPlatformY != 0f && lastPlatformX != 0f)
            {
                transform.position = new Vector3(lastPlatformX, lastPlatformY, transform.position.z);
            }
            else
            {
                // If no last platform data, reset to a default position
                transform.position = new Vector3(0f, 5f, 0f);
            }
            rb.linearVelocity = Vector2.zero; // Reset velocity
        }
    }



    void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveInput * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
