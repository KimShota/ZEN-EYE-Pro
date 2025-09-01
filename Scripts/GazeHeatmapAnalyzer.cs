using System.Collections.Generic;
using UnityEngine;

public enum ImagePairLetter { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O }
public enum HeatmapMode { Frequency, Duration }

[System.Serializable]
public class CellIDCount
{
    public string cellID;
    public int count;

    public CellIDCount(string id, int value)
    {
        cellID = id;
        count = value;
    }
}

public class GazeHeatmapAnalyzer : MonoBehaviour
{
    [Header("Configuration")]
    public HeatmapMode mode = HeatmapMode.Frequency;
    public ImagePairLetter selectedImagePair = ImagePairLetter.M;
    public TextAsset gazeCSV;

    [Header("Output (Read-Only)")]
    public List<CellIDCount> results = new();

    void Start()
    {
        if (gazeCSV != null)
            AnalyzeGazeData();
    }

    void AnalyzeGazeData()
    {
        string[] lines = gazeCSV.text.Split('\n');
        string targetPair = "imagePair" + selectedImagePair.ToString();

        Dictionary<string, int> cellMap = new();
        string previousCellID = "";
        int currentStreak = 0;

        foreach (string line in lines)
        {
            if (!line.Contains(targetPair))
            {
                if (mode == HeatmapMode.Duration && previousCellID != "" && currentStreak > 0)
                {
                    if (!cellMap.ContainsKey(previousCellID))
                        cellMap[previousCellID] = currentStreak;
                    else
                        cellMap[previousCellID] += currentStreak;
                }

                previousCellID = "";
                currentStreak = 0;
                continue;
            }

            string[] tokens = line.Split(',');
            if (tokens.Length < 7) continue;

            string cellID = tokens[5].Trim();

            if (cellID == "cellIDNone") continue;

            if (mode == HeatmapMode.Frequency)
            {
                if (cellID != previousCellID)
                {
                    if (!cellMap.ContainsKey(cellID))
                        cellMap[cellID] = 1;
                    else
                        cellMap[cellID]++;
                }
            }
            else if (mode == HeatmapMode.Duration)
            {
                if (cellID == previousCellID)
                {
                    currentStreak++;
                }
                else
                {
                    if (previousCellID != "" && currentStreak > 0)
                    {
                        if (!cellMap.ContainsKey(previousCellID))
                            cellMap[previousCellID] = currentStreak;
                        else
                            cellMap[previousCellID] += currentStreak;
                    }

                    currentStreak = 1;
                    previousCellID = cellID;
                }
            }

            previousCellID = cellID;
        }

        if (mode == HeatmapMode.Duration && previousCellID != "" && currentStreak > 0)
        {
            if (!cellMap.ContainsKey(previousCellID))
                cellMap[previousCellID] = currentStreak;
            else
                cellMap[previousCellID] += currentStreak;
        }

        results.Clear();
        foreach (var kvp in cellMap)
        {
            results.Add(new CellIDCount(kvp.Key, kvp.Value));
            Debug.Log($"🟩 {kvp.Key} = {kvp.Value}");
        }
    }
}
