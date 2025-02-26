using UnityEngine;

public class GridObjectsInitializer : MonoBehaviour
{
    [SerializeField] private GridObjectsAsset gridObjectsAsset; // Shared dictionary
    [SerializeField] private Transform machinesContainer;       // Parent containing all machines
    [SerializeField] private Grid grid;                        // Unity Grid component

    void Start()
    {
        if (gridObjectsAsset == null)
        {
            Debug.LogError("[ERROR] GridObjectsAsset is missing in GridObjectsInitializer!");
            return;
        }

        // Clear the dictionary to prevent duplicate entries
        gridObjectsAsset.gridObjects.Clear();

        foreach (Transform machine in machinesContainer)
        {
            if (machine.CompareTag("machine")) // Ensure this object is a valid machine
            {
                Vector3Int cellPosition = grid.WorldToCell(machine.position);
                cellPosition.y = 0; // Ensure Y is always 0 for proper detection

                if (!gridObjectsAsset.gridObjects.ContainsKey(cellPosition))
                {
                    gridObjectsAsset.gridObjects.Add(cellPosition, machine.gameObject);
                    // Debug.Log($"[DEBUG] Machine registered at {cellPosition}");
                }
            }
        }

        Debug.Log("[DEBUG] GridObjects dictionary initialized with existing machines.");
    }
}