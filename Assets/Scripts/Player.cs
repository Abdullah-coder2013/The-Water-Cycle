using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private InputAction moveAction;
    [SerializeField] private float speed = 5f; // Speed of the player movement
    private MiniGame1Manager miniGame1Manager;
    private Animator animator;
    
    [Header("Audio")] 
    [SerializeField] private AudioClip hurtSound;

    [SerializeField] private AudioClip collectSound;
    private AudioSource playerAudioSource;
    public int collectednumber = 0;
    [SerializeField] private TextMeshProUGUI collectednumberText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");
        playerAudioSource = GetComponent<AudioSource>();
        miniGame1Manager = GameObject.Find("PrecipitationGameManager").GetComponent<MiniGame1Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            collectednumberText.text = "Score: " + collectednumber.ToString();
        }
        catch
        {
            // ignored
        }

        var moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput != Vector2.zero || transform.position.x is < 11f and > -11f)
        {
            var moveDirection = new Vector3(moveInput.x,0,0);
            transform.position += moveDirection * (Time.deltaTime * speed); // Adjust speed as needed
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            collectednumber--;
            animator.SetBool("isHurt", true);
            playerAudioSource.PlayOneShot(hurtSound);
            StartCoroutine(StopHurt());
            if (miniGame1Manager.speed >11f) 
                miniGame1Manager.speed -= 3f;
        }
        else if (collision.gameObject.CompareTag("Collectible"))
        {
            playerAudioSource.PlayOneShot(collectSound);
            collectednumber++;
        }
    }

    private IEnumerator StopHurt()
    {
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("isHurt", false);
    }
}
