using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCIty : MonoBehaviour
{
	private InputAction moveAction;
	private InputAction jumpAction;
	private MiniGameCity miniGameForestManager;
	public bool onStart=true;
	public float speed = 2f;
	public float jumpForce = 5f; // Adjust jump force as needed
	private Rigidbody2D rb;
	public Transform startPos;
	private bool isJumping = false;
	private bool firstTime = true;
	private bool reached;
	private bool finished;
	
	[Header("Information System Integration")]
	[SerializeField] private bool enableFactDisplay = true;
	[SerializeField] private float factStartDelay = 2f;
	
	[Header("Audio")]
	public AudioClip jumpSound;
	private AudioSource playerAudioSource;
	public AudioClip moveSound;
	public AudioClip hurtSound;
	public AudioClip fallSound;

	private LevelLoader levelLoader;

	private void Start()
	{
		Information.OnFactSequenceCompleted += InformationOnOnFactSequenceCompleted;
		moveAction = InputSystem.actions.FindAction("Move");
		jumpAction = InputSystem.actions.FindAction("Jump");
		rb = GetComponent<Rigidbody2D>();
		if (enableFactDisplay && Information.Instance != null)
		{
			StartCoroutine(StartFactsWithDelay());
		}
		playerAudioSource = GetComponent<AudioSource>();
		levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
	}

	private void InformationOnOnFactSequenceCompleted()
	{
		Debug.Log($"[PlayerCIty] OnFactSequenceCompleted triggered. Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}, reached: {reached}, finished: {finished}");
		finished = true;
	}

	private IEnumerator StartFactsWithDelay()
	{
		yield return new WaitForSeconds(factStartDelay);
		Information.Instance.StartFactSequence(WaterCycleStage.Drainage);
	}
    
	private void OnDestroy()
	{
		Debug.Log("[PlayerCIty] OnDestroy called - unsubscribing from events");
		// Unsubscribe from events
		Information.OnFactSequenceCompleted -= InformationOnOnFactSequenceCompleted;
		
		// Stop fact sequence when minigame ends
		if (Information.Instance != null && Information.Instance.IsPlayingSequence())
		{
			Information.Instance.StopFactSequence();
		}
	}

	private void Update()
	{
		if (reached && finished)
			{
				Debug.Log("[PlayerCIty] Both reached and finished are true. Loading River scene.");
				levelLoader.LoadLevel("River");
			}
		if (reached)
		{
			return;
		}
		var moveInput = moveAction.ReadValue<Vector2>();
		if (moveInput != Vector2.zero)
		{
			if (!playerAudioSource.isPlaying)
				playerAudioSource.PlayOneShot(moveSound);
			var moveDirection = new Vector3(moveInput.x, moveInput.y, 0);
			transform.position += moveDirection * (Time.deltaTime * speed); // Adjust speed as needed
		}
		if (jumpAction.triggered && !isJumping)
		{
			playerAudioSource.PlayOneShot(jumpSound);
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Adjust jump force as needed
			isJumping = true;
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
			if (firstTime)
				rb.linearVelocity = Vector2.zero;
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
				reached = true;
			}
	}
}