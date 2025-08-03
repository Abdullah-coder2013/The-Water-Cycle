using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator animator;
    public float transitionTime = 1f;


    // Update is called once per frame
    public void LoadLevel(string sceneName)
    {
        Debug.Log($"[LevelLoader] LoadLevel called for scene: {sceneName}");
        if (this == null)
        {
            Debug.LogError("[LevelLoader] LevelLoader is null when trying to load scene!");
            return;
        }
        
        if (animator == null)
        {
            Debug.LogError("[LevelLoader] Animator is null! Cannot perform scene transition.");
            return;
        }
        
        Debug.Log($"[LevelLoader] Starting LoadScene coroutine for: {sceneName}");
        StartCoroutine(LoadScene(sceneName));
    }
    private IEnumerator LoadScene(string sceneName)
    {
        Debug.Log($"[LevelLoader] LoadScene coroutine started for: {sceneName}");
        
        // Check if we're still valid before starting animation
        if (this == null)
        {
            Debug.LogError("[LevelLoader] LevelLoader is null at start of coroutine!");
            yield break;
        }
        
        if (animator == null)
        {
            Debug.LogError("[LevelLoader] Animator is null at start of coroutine!");
            yield break;
        }
        
        Time.timeScale = 1f; // Ensure time scale is normal before starting transition
        Debug.Log("[LevelLoader] Time scale set to 1f");
        
        // Start the transition animation
        Debug.Log("[LevelLoader] Starting transition animation");
        try
        {
            animator.SetTrigger("Start");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LevelLoader] Exception setting animator trigger: {e.Message}");
            yield break;
        }

        // Wait for the transition to complete
        Debug.Log($"[LevelLoader] Waiting {transitionTime} seconds for transition");
        yield return new WaitForSeconds(transitionTime);

        // Final null check before scene load
        if (this == null)
        {
            Debug.LogError("[LevelLoader] LevelLoader became null during transition wait!");
            yield break;
        }

        // Load the new scene asynchronously
        Debug.Log($"[LevelLoader] Loading scene: {sceneName} (this will destroy current LevelLoader)");
        try
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            if (asyncLoad == null)
            {
                Debug.LogError($"[LevelLoader] Failed to start async load for scene: {sceneName}");
                yield break;
            }
            
            Debug.Log($"[LevelLoader] Scene load operation started successfully for: {sceneName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LevelLoader] Exception during scene load: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public bool IsTransitioning()
    {
        if (this == null || animator == null)
        {
            Debug.LogWarning("[LevelLoader] LevelLoader or animator is null in IsTransitioning check");
            return false;
        }
        
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