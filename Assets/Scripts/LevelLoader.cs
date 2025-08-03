using System.Collections;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public Animator animator;
    public float transitionTime = 1f;

    // Update is called once per frame
    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }
    private IEnumerator LoadScene(string sceneName)
    {
        // Start the transition animation
        animator.SetTrigger("Start");

        // Wait for the transition to complete
        yield return new WaitForSeconds(transitionTime);

        // Load the new scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public bool IsTransitioning()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fade_Endf"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}