using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrecomputedPool
{
    public int rolls { get; set; }
    public List<Entry> precomputedEntries { get; set; } = new List<Entry>();
    public List<Condition> conditions { get; set; } = new List<Condition>();
    public List<float> cumulativeWeights { get; private set; } = new List<float>();
    public float totalWeights { get; private set; }

    public void PrecomputeWeights()
    {
        totalWeights = 0;
        cumulativeWeights.Clear();

        foreach (var entry in precomputedEntries)
        {
            totalWeights += entry.weight;
            cumulativeWeights.Add(totalWeights);
        }
    }
}


