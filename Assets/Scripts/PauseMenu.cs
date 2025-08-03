using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private InputAction pauseAction;
    [Header("Audio")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioSource buttonClickSource;
    private void Start()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
        buttonClickSource = GetComponent<AudioSource>();
    }
    public void ResumeGame()
    {

        Time.timeScale = 1f; // Resume the game
        buttonClickSource.PlayOneShot(buttonClickClip); // Play button click sound
        transform.GetChild(0).gameObject.SetActive(false); // Hide the pause menu
    }
    public void PauseGame()
    {
        buttonClickSource.PlayOneShot(buttonClickClip); // Play button click sound
        transform.GetChild(0).gameObject.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Pause the game
        
    }
    public void ButtonCLick()
    {
        buttonClickSource.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch for variety
        buttonClickSource.PlayOneShot(buttonClickClip);
    }
    private void Update()
    {
        if (pauseAction.triggered)
        {
            if (transform.GetChild(0).gameObject.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}
