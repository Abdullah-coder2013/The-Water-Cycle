using System.Collections.Generic;
using UnityEngine;

// ScriptableObject for managing stage-specific facts
[CreateAssetMenu(fileName = "StageFactsData", menuName = "Water Cycle/Stage Facts")]
public class StageFactsData : ScriptableObject
{
    [Header("Stage Information")]
    public WaterCycleStage stage;
    public string stageName;
    
    [Header("Facts for this Stage")]
    public List<FactData> facts = new List<FactData>();
    
    [Header("Default Settings")]
    public float defaultFactDuration = 5f;
    public float timeBetweenFacts = 1f;
}

