using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;                         // Unity's Grid component
    [SerializeField] private Transform machinesContainer;       // Parent holding all machine objects
    [SerializeField] private Material testGhostMat;             // Ghost Mat

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
            // Ensure the spawned machine keeps its original Y position
            float originalY = prefab.transform.position.y;
            spawnPosition.y = originalY;

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

            // Store the picked-up machine reference
            heldMachinePrefab = machineToPick;
            machineToPick.SetActive(false); // Hide the original machine
            gridObjects.Remove(indicatorCellPosition); // Remove from grid tracking

            // Get the original Y position of the prefab
            float originalY = heldMachinePrefab.transform.position.y;

            // Create a ghost version at the player's indicator position
            Vector3 ghostSpawnPosition = grid.GetCellCenterWorld(grid.WorldToCell(indicatorWorldPosition));
            ghostSpawnPosition.y = originalY; // Keep the original Y height

            GameObject ghostMachine = Instantiate(heldMachinePrefab, ghostSpawnPosition, Quaternion.identity, machinesContainer);
            ghostMachine.name = heldMachinePrefab.name + "_Ghost"; // Differentiate from original
            ghostMachine.SetActive(true);

            // Apply ghost material effect
            ApplyGhostMaterial(ghostMachine, testGhostMat);
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

    private void ApplyGhostMaterial(GameObject targetObject, Material testGhostMat)
    {
        if (targetObject == null || testGhostMat == null) return;

        Renderer targetRenderer = targetObject.GetComponent<Renderer>();
        if (targetRenderer == null || targetRenderer.sharedMaterial == null) return;

        // Get the original material from the object
        Material originalMaterial = targetRenderer.sharedMaterial;

        // Create a new instance of the ghost material to prevent affecting other objects
        Material newGhostMaterial = new Material(testGhostMat);

        // Check if the original material has an Albedo color and transfer it directly
        if (originalMaterial.HasProperty("_Color")) // Standard Shader uses _BaseColor
        {
            Color originalColor = originalMaterial.GetColor("_Color");

            // Directly transfer the color to both BaseColor and GhostColor
            if (newGhostMaterial.HasProperty("_Base_Color")) // Ensure GhostColor exists in shader
            {
                newGhostMaterial.SetColor("_Base_Color", originalColor);
            } else {
                Debug.Log("not found ghostmat's base color");
            }
            if (newGhostMaterial.HasProperty("_Ghost_Color")) // Ensure GhostColor exists in shader
            {
                newGhostMaterial.SetColor("_Ghost_Color", originalColor);
            } else {
                Debug.Log("not found ghostmat's ghost color");
            }
        }

        // Assign the new ghost material to the target object
        targetRenderer.material = newGhostMaterial;
    }
}
