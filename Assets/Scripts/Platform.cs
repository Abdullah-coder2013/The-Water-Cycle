using UnityEngine;

public class Platform : MonoBehaviour
{
    public float jumpForce = 10f;
    public AudioClip jumpSound;
    private AudioSource audioSource;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y <= 0f)
        {
            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 velocity = rb.linearVelocity;
                velocity.y = jumpForce;
                rb.linearVelocity = velocity;
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            // Play jump sound
            audioSource = collision.gameObject.GetComponent<AudioSource>();
            audioSource.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch for variety
            audioSource.PlayOneShot(jumpSound);
            PlayerPrefs.SetFloat("LastPlatformY", collision.gameObject.transform.position.y);
            PlayerPrefs.SetFloat("LastPlatformX", collision.gameObject.transform.position.x);
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        // if platform is below the camera, destroy it
        if (transform.position.y < Camera.main.transform.position.y - 10f)
        {
            Destroy(gameObject);
        }
    }
}