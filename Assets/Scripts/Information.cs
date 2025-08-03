using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Audio;

// Enum for different water cycle stages
public enum WaterCycleStage
{
    Intro,
    Precipitation,
    Drainage,
    River,
    Ocean,
    Evaporation,
    Condensation,
}

// Data structure for individual facts



// Main Information Manager class
public class Information : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI factDisplayText;
    [SerializeField] private GameObject factPanel;
    [SerializeField] private CanvasGroup factPanelCanvasGroup;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup narrationAudioGroup;
    
    [Header("Stage Facts Data")]
    [SerializeField] private List<StageFactsData> stageFactsData = new List<StageFactsData>();
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    // Singleton instance
    public static Information Instance { get; private set; }
    
    // Current state
    private WaterCycleStage currentStage;
    private StageFactsData currentStageData;
    private int currentFactIndex = 0;
    private bool isPlayingSequence = false;
    private Coroutine factSequenceCoroutine;
    
    // Events
    public static event Action<WaterCycleStage> OnStageChanged;
    public static event Action<FactData, int> OnFactDisplayed;
    public static event Action OnFactSequenceCompleted;
    public static event Action OnFactSequenceStarted;
    
    private void Awake()
    {
        Debug.Log($"[Information] Awake called in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        // Scene-specific singleton pattern - each scene gets its own Information instance
        if (Instance == null)
        {
            Debug.Log("[Information] Creating new scene-specific instance");
            Instance = this;
            InitializeComponents();
        }
        else
        {
            Debug.Log("[Information] Destroying duplicate instance in same scene");
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        Debug.Log($"[Information] OnDestroy called in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        // Clear singleton reference when this scene's instance is destroyed
        if (Instance == this)
        {
            Instance = null;
        }
        
        // Stop any running coroutines
        if (factSequenceCoroutine != null)
        {
            StopCoroutine(factSequenceCoroutine);
        }
    }
    
    private void InitializeComponents()
    {
        // Initialize audio source if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.outputAudioMixerGroup = narrationAudioGroup;
        }
        
        // Initialize UI components if not assigned
        if (factPanel != null && factPanelCanvasGroup == null)
        {
            factPanelCanvasGroup = factPanel.GetComponent<CanvasGroup>();
            if (factPanelCanvasGroup == null)
            {
                factPanelCanvasGroup = factPanel.AddComponent<CanvasGroup>();
            }
        }
        
        // Hide fact panel initially
        if (factPanel != null)
        {
            factPanel.SetActive(false);
        }
    }
    
    #region Public Methods
    
    /// <summary>
    /// Start displaying facts for a specific water cycle stage
    /// </summary>
    /// <param name="stage">The water cycle stage to display facts for</param>
    public void StartFactSequence(WaterCycleStage stage)
    {
        Debug.Log($"[Information] StartFactSequence called for stage: {stage}");
        
        if (isPlayingSequence)
        {
            Debug.Log("[Information] Already playing sequence, stopping current one");
            StopFactSequence();
        }
        
        currentStage = stage;
        currentStageData = GetStageFactsData(stage);
        
        if (currentStageData == null || currentStageData.facts.Count == 0)
        {
            Debug.LogWarning($"No facts data found for stage: {stage}. Available stages: {string.Join(", ", stageFactsData.ConvertAll(data => data.stage.ToString()))}");
            // Complete immediately if no facts data
            CompleteFactSequence();
            return;
        }
        
        Debug.Log($"[Information] Found {currentStageData.facts.Count} facts for stage {stage}");
        currentFactIndex = 0;
        isPlayingSequence = true;
        
        try
        {
            OnStageChanged?.Invoke(stage);
            OnFactSequenceStarted?.Invoke();
            Debug.Log("[Information] Stage events invoked successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Information] Exception during stage event invocation: {e.Message}");
        }
        
        factSequenceCoroutine = StartCoroutine(PlayFactSequence());
        Debug.Log("[Information] Fact sequence coroutine started");
    }
    
    /// <summary>
    /// Stop the current fact sequence
    /// </summary>
    public void StopFactSequence()
    {
        if (factSequenceCoroutine != null)
        {
            StopCoroutine(factSequenceCoroutine);
            factSequenceCoroutine = null;
        }
        
        isPlayingSequence = false;
        
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        HideFactPanel();
    }
    
    /// <summary>
    /// Pause the current fact sequence
    /// </summary>
    public void PauseFactSequence()
    {
        if (isPlayingSequence && factSequenceCoroutine != null)
        {
            StopCoroutine(factSequenceCoroutine);
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }
    
    /// <summary>
    /// Resume the paused fact sequence
    /// </summary>
    public void ResumeFactSequence()
    {
        if (isPlayingSequence && factSequenceCoroutine == null)
        {
            if (audioSource != null)
            {
                audioSource.UnPause();
            }
            factSequenceCoroutine = StartCoroutine(PlayFactSequence());
        }
    }
    
    /// <summary>
    /// Skip to the next fact in the sequence
    /// </summary>
    public void SkipToNextFact()
    {
        if (isPlayingSequence && currentStageData != null)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            
            currentFactIndex++;
            if (currentFactIndex >= currentStageData.facts.Count)
            {
                CompleteFactSequence();
            }
        }
    }
    
    /// <summary>
    /// Get facts data for a specific stage
    /// </summary>
    /// <param name="stage">The water cycle stage</param>
    /// <returns>StageFactsData for the specified stage</returns>
    public StageFactsData GetStageFactsData(WaterCycleStage stage)
    {
        return stageFactsData.Find(data => data.stage == stage);
    }
    
    /// <summary>
    /// Check if a fact sequence is currently playing
    /// </summary>
    /// <returns>True if a sequence is playing</returns>
    public bool IsPlayingSequence()
    {
        return isPlayingSequence;
    }
    
    /// <summary>
    /// Get the current stage
    /// </summary>
    /// <returns>Current water cycle stage</returns>
    public WaterCycleStage GetCurrentStage()
    {
        return currentStage;
    }
    
    /// <summary>
    /// Get the current fact index
    /// </summary>
    /// <returns>Index of the currently displayed fact</returns>
    public int GetCurrentFactIndex()
    {
        return currentFactIndex;
    }
    
    #endregion
    
    #region Private Methods
    
    private IEnumerator PlayFactSequence()
    {
        while (currentFactIndex < currentStageData.facts.Count && isPlayingSequence)
        {
            FactData currentFact = currentStageData.facts[currentFactIndex];
            
            // Wait for delay before showing fact
            if (currentFact.delayBeforeShow > 0)
            {
                yield return new WaitForSeconds(currentFact.delayBeforeShow);
            }
            
            // Display the fact
            yield return StartCoroutine(DisplayFact(currentFact));
            
            // Wait between facts
            if (currentFactIndex < currentStageData.facts.Count - 1)
            {
                yield return new WaitForSeconds(currentStageData.timeBetweenFacts);
            }
            
            currentFactIndex++;
        }
        
        CompleteFactSequence();
    }
    
    private IEnumerator DisplayFact(FactData fact)
    {
        // Show fact panel and text
        ShowFactPanel();
        SetFactText(fact.factText);
        
        // Trigger event
        OnFactDisplayed?.Invoke(fact, currentFactIndex);
        
        // Play audio if available
        if (fact.narrationClip != null && audioSource != null)
        {
            audioSource.clip = fact.narrationClip;
            audioSource.Play();
        }
        
        // Determine display duration
        float displayDuration = GetFactDisplayDuration(fact);
        
        // Wait for the appropriate duration
        if (fact.waitForAudio && fact.narrationClip != null && audioSource != null)
        {
            // Wait for audio to finish or display duration, whichever is longer
            float audioLength = fact.narrationClip.length;
            float waitTime = Mathf.Max(displayDuration, audioLength);
            yield return new WaitForSeconds(waitTime);
        }
        else
        {
            // Just wait for display duration
            yield return new WaitForSeconds(displayDuration);
        }
    }
    
    private float GetFactDisplayDuration(FactData fact)
    {
        if (fact.displayDuration > 0)
        {
            return fact.displayDuration;
        }
        else if (fact.narrationClip != null)
        {
            return fact.narrationClip.length;
        }
        else
        {
            return currentStageData.defaultFactDuration;
        }
    }
    
    private void ShowFactPanel()
    {
        if (factPanel != null)
        {
            factPanel.SetActive(true);
            if (factPanelCanvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(factPanelCanvasGroup, 0f, 1f, fadeInDuration));
            }
        }
    }
    
    private void HideFactPanel()
    {
        if (factPanel != null && factPanelCanvasGroup != null)
        {
            StartCoroutine(FadeCanvasGroup(factPanelCanvasGroup, 1f, 0f, fadeOutDuration, () => {
                factPanel.SetActive(false);
            }));
        }
        else if (factPanel != null)
        {
            factPanel.SetActive(false);
        }
    }
    
    private void SetFactText(string text)
    {
        if (factDisplayText != null)
        {
            factDisplayText.text = text;
        }
    }
    
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration, Action onComplete = null)
    {
        if (canvasGroup == null)
        {
            onComplete?.Invoke();
            yield break;
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration && canvasGroup != null)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = endAlpha;
        }
        onComplete?.Invoke();
    }
    
    private void CompleteFactSequence()
    {
        Debug.Log($"[Information] CompleteFactSequence called. Current stage: {currentStage}, Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        isPlayingSequence = false;
        factSequenceCoroutine = null;
        
        HideFactPanel();
        
        Debug.Log("[Information] About to invoke OnFactSequenceCompleted event");
        try
        {
            if (OnFactSequenceCompleted != null)
            {
                Debug.Log($"[Information] OnFactSequenceCompleted has {OnFactSequenceCompleted.GetInvocationList().Length} subscribers");
                OnFactSequenceCompleted.Invoke();
                Debug.Log("[Information] OnFactSequenceCompleted event invoked successfully");
            }
            else
            {
                Debug.Log("[Information] OnFactSequenceCompleted event is null - no subscribers");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Information] Exception during OnFactSequenceCompleted event: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }
    
    #endregion
    
    #region Editor Helper Methods
    
    #if UNITY_EDITOR
    [ContextMenu("Test Precipitation Facts")]
    private void TestPrecipitationFacts()
    {
        StartFactSequence(WaterCycleStage.Precipitation);
    }
    
    [ContextMenu("Stop Current Sequence")]
    private void StopCurrentSequence()
    {
        StopFactSequence();
    }
    #endif
    
    #endregion
}
