using System.Collections.Generic;
using UnityEngine;

public class HeatmapGridSpawner : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject gridPrefab;
    public int rows = 16;
    public int columns = 16;
    public int startingIndex = 1;

    [Header("Heatmap Data")]
    public Dictionary<string, int> heatmapData = new(); // key = "Grid_151", value = count
    public Gradient colorGradient;

    private GameObject[,] gridArray;
    //private bool colorizeAfterSpawn = false;

    void Start()
    {
        SpawnGrid();
        // Color will apply in next frame when grids exist
        Invoke(nameof(DelayedColorize), 0.1f);
    }

    void DelayedColorize()
    {
        if (heatmapData.Count > 0)
        {
            ApplyHeatmapColor();
        }
    }

    public void SpawnGrid()
    {
        if (gridArray != null)
        {
            foreach (var obj in gridArray)
                if (obj != null) Destroy(obj);
        }

        gridArray = new GameObject[rows, columns];

        MeshFilter meshFilter = gridPrefab.GetComponent<MeshFilter>();
        Vector3 meshSize = meshFilter.sharedMesh.bounds.size;
        Vector3 prefabScale = gridPrefab.transform.localScale;

        float cellWidth = meshSize.x * prefabScale.x;
        float cellHeight = meshSize.y * prefabScale.y;

        int globalIndex = startingIndex;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 localPos = new Vector3(x * cellWidth, -y * cellHeight, 0);
                GameObject gridObj = Instantiate(gridPrefab, transform);
                gridObj.transform.localPosition = localPos;

                string gridName = $"Grid_{globalIndex}";
                gridObj.name = gridName;

                gridArray[x, y] = gridObj;
                globalIndex++;
            }
        }
    }

    public void LoadHeatmapFromAnalyzer(List<CellIDCount> results)
    {
        Debug.Log("✅ LoadHeatmapFromAnalyzer CALLED");

        if (results == null || results.Count == 0)
        {
            Debug.LogWarning("🚨 No results passed in from analyzer.");
            return;
        }

        heatmapData.Clear();
        int maxCount = 0;

        foreach (var entry in results)
        {
            string cellID = entry.cellID;
            if (!cellID.StartsWith("cellIDGrid_")) continue;

            int id = int.Parse(cellID.Substring(11));
            string unityGridName = $"Grid_{((id - 1) % 256) + 1}";

            if (!heatmapData.ContainsKey(unityGridName))
                heatmapData[unityGridName] = entry.count;
            else
                heatmapData[unityGridName] += entry.count;

            if (entry.count > maxCount)
                maxCount = entry.count;

            Debug.Log($"📦 {cellID} → {unityGridName} = {entry.count}");
        }

        Debug.Log($"🔥 Total Mapped Grids: {heatmapData.Count}, Max Count = {maxCount}");
    }

    public void ApplyHeatmapColor()
    {
        Debug.Log("🎨 Applying heatmap colors...");

        if (heatmapData == null || heatmapData.Count == 0)
        {
            Debug.LogWarning("🚨 heatmapData is empty.");
            return;
        }

        int maxCount = 0;
        foreach (var val in heatmapData.Values)
            if (val > maxCount) maxCount = val;

        foreach (Transform grid in transform)
        {
            string name = grid.name;
            Renderer r = grid.GetComponent<Renderer>();
            if (!r) continue;

            if (heatmapData.TryGetValue(name, out int count))
            {
                float intensity = (float)count / maxCount;
                Color color = Color.Lerp(Color.blue, Color.red, intensity);
                r.material.color = color;
                r.material.SetColor("_BaseColor", color);
                Debug.Log($"🟥 {name} matched → Count: {count}, Color: {color}");
            }
            else
            {
                Color gray = new Color(0.3f, 0.3f, 0.3f);
                r.material.color = gray;
                r.material.SetColor("_BaseColor", gray);
                Debug.Log($"⬜ {name} not matched → colored gray.");
            }
        }
    }
}
