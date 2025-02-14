using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;                         // Unity's Grid component
    [SerializeField] private Transform machinesContainer;       // Parent holding all machine objects

    private Dictionary<Vector3Int, GameObject> gridObjects = new Dictionary<Vector3Int, GameObject>();
    private GameObject heldMachinePrefab = null;                // Store the picked-up machine prefab

    void Start()
    {
        foreach (Transform machine in machinesContainer)
        {
            if (machine.CompareTag("machine"))
            {
                Vector3Int cellPosition = grid.WorldToCell(machine.position);
                cellPosition.y = 0; // Ensure Y is always 0 for proper detection

                if (!gridObjects.ContainsKey(cellPosition))
                {
                    gridObjects.Add(cellPosition, machine.gameObject);
                }
            }
        }
    }

    // Method to spawn a new machine prefab into the grid
    public void TrySpawn(GameObject prefab, Vector3 indicatorWorldPosition)
    {
        if (heldMachinePrefab != null)
        {
            Debug.Log("Cannot spawn: Already holding a machine.");
            return;
        }

        Vector3Int spawnCellPosition = grid.WorldToCell(indicatorWorldPosition);
        spawnCellPosition.y = 0; // Ensure stored grid position uses Y = 0 for consistency

        if (!gridObjects.ContainsKey(spawnCellPosition))
        {
            Vector3 spawnPosition = grid.GetCellCenterWorld(spawnCellPosition);
            spawnPosition.y = 0.25f; // Fixed Y height for all machines

            GameObject newMachine = Instantiate(prefab, spawnPosition, Quaternion.identity, machinesContainer);

            if (!newMachine.CompareTag("machine"))
            {
                Debug.LogError($"Spawned prefab '{newMachine.name}' is missing the 'machine' tag!");
                Destroy(newMachine);
                return;
            }

            gridObjects.Add(spawnCellPosition, newMachine);
            Debug.Log($"Spawned machine: {newMachine.name} at {spawnCellPosition}");
        }
        else
        {
            Debug.Log("Spawn failed: Cell is already occupied.");
        }
    }

    // This method is called when the player presses the interact button (E)
    public void TryInteract(Vector3 indicatorWorldPosition)
    {
        if (heldMachinePrefab != null)
        {
            Debug.Log("Cannot pick up: Already holding a machine.");
            return;
        }

        Vector3Int indicatorCellPosition = grid.WorldToCell(indicatorWorldPosition);
        indicatorCellPosition.y = 0; // Ignore Y-axis to detect machines at any height

        if (gridObjects.ContainsKey(indicatorCellPosition))
        {
            GameObject machineToPick = gridObjects[indicatorCellPosition];
            Debug.Log("Picked up: " + machineToPick.name);

            heldMachinePrefab = machineToPick;
            machineToPick.SetActive(false); // Temporarily hide the machine
            gridObjects.Remove(indicatorCellPosition);
        }
        else
        {
            Debug.Log("No machine at this position.");
        }
    }

    // This method is called when the player presses the drop button (C)
    public void TryDrop(Vector3 indicatorWorldPosition)
    {
        if (heldMachinePrefab != null)
        {
            Vector3Int dropCellPosition = grid.WorldToCell(indicatorWorldPosition);
            dropCellPosition.y = 0; // Ensure stored grid position uses Y = 0 for consistency


            if (!gridObjects.ContainsKey(dropCellPosition))
            {
                // Get the correct grid center position (actually redundant could be DELETE)
                Vector3 dropPosition = grid.GetCellCenterWorld(dropCellPosition);

                // Ensure the dropped machine uses its original Y height (actually redundant could be DELETE)
                float correctY = heldMachinePrefab.transform.position.y;
                dropPosition.y = correctY;

                // Instantiate at the correct position and height
                GameObject newMachine = Instantiate(heldMachinePrefab, dropPosition, Quaternion.identity, machinesContainer);
                newMachine.name = heldMachinePrefab.name;  // Remove (Clone) suffix
                newMachine.SetActive(true);

                gridObjects.Add(dropCellPosition, newMachine);
                Debug.Log("Dropped machine at: " + dropCellPosition);

                // Destroy the previously held machine to avoid clutter
                Destroy(heldMachinePrefab);
                heldMachinePrefab = null;  // Clear the reference after dropping
            }
            else
            {
                Debug.Log("There's already a machine at this position.");
            }
        }
        else
        {
            Debug.Log("No machine to drop.");
        }
    }

}
