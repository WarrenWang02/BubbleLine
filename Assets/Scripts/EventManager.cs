using UnityEditor.PackageManager;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GridObjectsAsset gridObjectsAsset; // Asset storing machine dictionary
    [SerializeField] private Grid grid; // Unity Grid component
    [SerializeField] private Transform machinesContainer; // Main container for active machines
    [SerializeField] private Transform tutorial1Container; 

    /// <summary>
    /// Enables a machine container, and registers them in the asset dictionary.
    /// </summary>
    private void EnableAndRegisterMachines(Transform Container)
    {
        Transform container = Container;
        
        if (container == null)
        {
            Debug.LogWarning($"[WARNING] Machine container '{Container.name}' not found.");
            return;
        }

        // Activate the container if it's inactive
        if (!container.gameObject.activeSelf)
        {
            container.gameObject.SetActive(true);
        }

        foreach (Transform machine in container)
        {
            if (machine.CompareTag("machine")) // Ensure it's a valid machine
            {
                Vector3Int cellPosition = grid.WorldToCell(machine.position);
                cellPosition.y = 0; // Keep Y consistent

                if (!gridObjectsAsset.gridObjects.ContainsKey(cellPosition))
                {
                    gridObjectsAsset.gridObjects.Add(cellPosition, machine.gameObject);
                }
            }
        }

        Debug.Log($"[DEBUG] Machines from '{Container.name}' enabled, and registered.");
    }

    /// <summary>
    /// Removes all machines from a container and updates the dictionary in the asset.
    /// </summary>
    private void RemoveMachinesFromContainer(Transform Container)
    {
        Transform container = Container;

        if (container == null)
        {
            Debug.LogWarning($"[WARNING] Machine container '{Container.name}' not found.");
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

        Debug.Log($"[DEBUG] Machines from '{Container.name}' removed from the dictionary.");
    }

    public void Tutorial1Trigger()
    {
        EnableAndRegisterMachines(tutorial1Container);
    }
}
