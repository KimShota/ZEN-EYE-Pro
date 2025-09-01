using UnityEngine;

public class GridCellSetup : MonoBehaviour
{
    [Header("Prefix for naming buttons (e.g., PortraitLeft)")]
    public string prefix = "PortraitLeft";

    [Header("Add collider if missing?")]
    public bool addCollider = true;

    void Start()
    {
        int index = 0;
        foreach (Transform child in transform)
        {
            string newName = $"{prefix}_{index}";
            child.name = newName;

            if (addCollider && child.GetComponent<Collider>() == null)
            {
                child.gameObject.AddComponent<BoxCollider>();
            }

            index++;
        }

        Debug.Log($"✅ Renamed {index} buttons under {prefix}_* and added colliders.");
    }
}
