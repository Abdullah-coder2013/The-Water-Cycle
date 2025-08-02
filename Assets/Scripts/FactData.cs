using UnityEngine;

[System.Serializable]
public class FactData
{
    [Header("Fact Content")]
    [TextArea(3, 6)]
    public string factText;
    
    [Header("Audio")]
    public AudioClip narrationClip;
    
    [Header("Timing Settings")]
    [Tooltip("Duration to display this fact (in seconds). If 0, will use audio clip length or default duration")]
    public float displayDuration = 0f;
    
    [Tooltip("Wait for audio to finish before proceeding to next fact")]
    public bool waitForAudio = true;
    
    [Tooltip("Delay before showing this fact (in seconds)")]
    public float delayBeforeShow = 0f;
}