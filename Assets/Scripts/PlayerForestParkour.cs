using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerForestParkour : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction jumpAction;
    private MiniGameForestManager miniGameForestManager;
    public bool onStart=true;
    public float speed = 2f;
    public float jumpForce = 5f; // Adjust jump force as needed
    private Rigidbody2D rb;
    public Transform startPos;
    private bool isJumping = false;
    
    [Header("Audio")]
    public AudioClip jumpSound;
    private AudioSource playerAudioSource;
    public AudioClip moveSound;
    public AudioClip hurtSound;
    public AudioClip fallSound;


    private void Start()
    {
        miniGameForestManager = GameObject.Find("MiniGame2ManagerForest").GetComponent<MiniGameForestManager>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();
        playerAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput != Vector2.zero)
        {
            var moveDirection = new Vector3(moveInput.x, moveInput.y, 0);
            transform.position += moveDirection * (Time.deltaTime * speed); // Adjust speed as needed
            if (!playerAudioSource.isPlaying)
            {
                playerAudioSource.PlayOneShot(moveSound);
            }
        }
        if (jumpAction.triggered && !isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Adjust jump force as needed
            playerAudioSource.PlayOneShot(jumpSound);
            isJumping = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (onStart)
        {
            rb.linearVelocity = Vector2.zero;
            playerAudioSource.PlayOneShot(fallSound);
            miniGameForestManager.TurnPlayerintoWater();
        }
        else
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                isJumping = false; // Reset jumping state when touching the ground
            }
            if (other.gameObject.CompareTag("Obstacle"))
            {
                playerAudioSource.PlayOneShot(hurtSound);
                transform.position = startPos.position;
            }
            if (other.gameObject.CompareTag("Finish"))
            {
                SceneManager.LoadScene("River");
            }
        }
    }
}
