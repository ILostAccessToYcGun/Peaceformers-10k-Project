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
    /// Pass in the name of the variable you want to change, and then by how much.
    /// Just make it a negative number to subtract
    /// </summary>
    /// <param name="name">HP, ATK, SPD, INV_SIZE</param>
    /// <param name="amount"></param>
    void ModifyStat(string name, float amount)
    {
        stats[name] += amount;
    }

    void ResetStats()
    {
        stats = originalStats;
    }
}
