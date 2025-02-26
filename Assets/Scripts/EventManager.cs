using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GridObjectsAsset gridObjectsAsset; // Asset storing machine dictionary
    [SerializeField] private Grid grid; // Unity Grid component
    [SerializeField] private Transform machinesContainer; // Main container for active machines

    /// <summary>
    /// Enables a machine container, moves machines to the main container, and registers them in the asset dictionary.
    /// </summary>
    public void EnableAndRegisterMachines(string containerName)
    {
        Transform container = GameObject.Find(containerName)?.transform;
        if (container == null)
        {
            Debug.LogWarning($"[WARNING] Machine container '{containerName}' not found.");
            return;
        }

        container.gameObject.SetActive(true); // Enable the container

        foreach (Transform machine in container)
        {
            if (machine.CompareTag("machine")) // Ensure it's a valid machine
            {
                // Move machine to the main machines container
                machine.SetParent(machinesContainer);

                Vector3Int cellPosition = grid.WorldToCell(machine.position);
                cellPosition.y = 0; // Keep Y consistent

                if (!gridObjectsAsset.gridObjects.ContainsKey(cellPosition))
                {
                    gridObjectsAsset.gridObjects.Add(cellPosition, machine.gameObject);
                }
            }
        }

        Debug.Log($"[DEBUG] Machines from '{containerName}' enabled, moved to container, and registered.");
    }

    /// <summary>
    /// Removes all machines from a container and updates the dictionary in the asset.
    /// </summary>
    public void RemoveMachinesFromContainer(string containerName)
    {
        Transform container = GameObject.Find(containerName)?.transform;
        if (container == null)
        {
            Debug.LogWarning($"[WARNING] Machine container '{containerName}' not found.");
            return;
        }

        // Remove all machines from the dictionary
        foreach (Transform machine in container)
        {
            if (machine.CompareTag("machine"))
            {
                Vector3Int cellPosition = grid.WorldToCell(machine.position);
                cellPosition.y = 0;

                if (gridObjectsAsset.gridObjects.ContainsKey(cellPosition))
                {
                    gridObjectsAsset.gridObjects.Remove(cellPosition);
                }
            }
        }

        Debug.Log($"[DEBUG] Machines from '{containerName}' removed from the dictionary.");
    }
}
