using UnityEngine;

public class HeatmapController : MonoBehaviour
{
    public GazeHeatmapAnalyzer analyzer;
    public HeatmapGridSpawner spawner;

    void Start()
    {
        if (analyzer != null && spawner != null)
        {
            spawner.LoadHeatmapFromAnalyzer(analyzer.results);
        }
    }
}
