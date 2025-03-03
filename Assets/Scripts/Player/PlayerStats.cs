using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class PlayerStats : MonoBehaviour
{
    [SerializedDictionary("Stat", "Value")]
    public SerializedDictionary<string, float> stats;

    [SerializedDictionary("Stat", "Value")]
    public SerializedDictionary<string, float> originalStats;

    void Awake()
    {
        originalStats = stats;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="amount"></param>
    void ModifyStat(int index, float amount)
    {
        stats[index] += amount;
    }

    void ResetStats()
    {
        stats = originalStats;
    }
}
