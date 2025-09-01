using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public GameObject gridPrefab;
    public int rows = 16;
    public int columns = 16;

    private GameObject[,] gridArray;

    [Tooltip("Starting index for naming this grid panel (e.g. 1, 257, 513, 769)")]
    public int startingIndex = 1;

    void Start()
    {
        SpawnGrid();
    }

    public void SpawnGrid()
    {
        // Clear existing grid
        if (gridArray != null)
        {
            foreach (var obj in gridArray)
            {
                if (obj != null) Destroy(obj);
            }
        }

        gridArray = new GameObject[rows, columns];

        // Get local mesh size of prefab
        MeshFilter meshFilter = gridPrefab.GetComponent<MeshFilter>();
        Vector3 meshSize = meshFilter.sharedMesh.bounds.size;

        // Use local scale of the prefab to get actual local spacing
        Vector3 prefabScale = gridPrefab.transform.localScale;
        float cellWidth = meshSize.x * prefabScale.x;
        float cellHeight = meshSize.y * prefabScale.y;

        int globalIndex = startingIndex;

        // Create each grid in local space
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 localPos = new Vector3(x * cellWidth, -y * cellHeight, 0);
                GameObject gridObj = Instantiate(gridPrefab, transform);
                gridObj.transform.localPosition = localPos;
                gridObj.name = $"Grid_{globalIndex++}";
            }
        }
    }
}
