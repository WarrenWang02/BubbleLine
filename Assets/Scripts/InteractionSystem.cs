using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;                         // Unity's Grid component
    [SerializeField] private Transform machinesContainer;       // Parent holding all machine objects
    [SerializeField] private Material testGhostMat;             // Ghost Mat

    private Dictionary<Vector3Int, GameObject> gridObjects = new Dictionary<Vector3Int, GameObject>();
    private GameObject heldMachinePrefab = null;                // Store the picked-up machine prefab
    private GridSystem gridsystem;

    void Start()
    {
        gridsystem = GetComponent<GridSystem>();
        
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
    public void TrySpawn(GameObject prefab)
    {
        if (heldMachinePrefab != null)
        {
            Debug.Log("Cannot spawn: Already holding a machine.");
            return;
        }

        // Prepare to drop the selected prefab instead of instantly spawning
        heldMachinePrefab = prefab;
        Debug.Log("Spawn prepared: " + prefab.name);

        GameObject ghostMachine = Instantiate(heldMachinePrefab);
        ghostMachine.SetActive(true);
        ghostMachine.GetComponent<Collider>().enabled = false;
        ghostMachine.GetComponent<Rigidbody>().detectCollisions = false;
        ApplyGhostMaterial(ghostMachine, testGhostMat);
        gridsystem.SetGhostPrefab(ghostMachine);
    }

    // This method is called when the player presses the interact button (E)
    public void TryInteract(Vector3 indicatorWorldPosition)
    {
        // If already holding an object, attempt to drop it instead
        if (heldMachinePrefab != null)
        {
            TryDrop(indicatorWorldPosition);
            return;
        }

        // Otherwise, try picking up a machine
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

            GameObject ghostMachine = Instantiate(heldMachinePrefab);
            ghostMachine.SetActive(true);
            ghostMachine.GetComponent<Collider>().enabled = false;
            ghostMachine.GetComponent<Rigidbody>().detectCollisions = false;
            ApplyGhostMaterial(ghostMachine, testGhostMat);
            gridsystem.SetGhostPrefab(ghostMachine);
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
                Vector3 dropPosition = grid.GetCellCenterWorld(dropCellPosition);
                float correctY = heldMachinePrefab.transform.position.y;
                dropPosition.y = correctY;

                GameObject newMachine = Instantiate(heldMachinePrefab, dropPosition, Quaternion.identity, machinesContainer);
                newMachine.name = heldMachinePrefab.name;
                newMachine.SetActive(true);

                gridObjects.Add(dropCellPosition, newMachine);
                Debug.Log("Dropped machine at: " + dropCellPosition);

                //destroying original prefab in the field
                if (heldMachinePrefab.scene.IsValid()) 
                {
                    Destroy(heldMachinePrefab); // Only destroy if it exists in the scene
                }
                // Reset held machine and remove ghost preview
                heldMachinePrefab = null;
                gridsystem.DeleteGhostPrefab();
            }
            else
            {
                Debug.Log("Drop failed: Cell already occupied.");
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
