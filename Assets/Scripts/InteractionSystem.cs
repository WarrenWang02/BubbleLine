using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;                         // Unity's Grid component
    [SerializeField] private Transform machinesContainer;       // Parent holding all machine objects

    private Dictionary<Vector3Int, GameObject> gridObjects = new Dictionary<Vector3Int, GameObject>();
    private GameObject heldMachinePrefab = null;                // Store the picked-up machine
    private string heldMachineName = "";                        // Store the name of the picked-up machine

    void Start()
    {
        // Populate the gridObjects dictionary with machines at the start
        foreach (Transform machine in machinesContainer)
        {
            if (machine.CompareTag("machine"))
            {
                Vector3Int cellPosition = grid.WorldToCell(machine.position);
                if (!gridObjects.ContainsKey(cellPosition))
                {
                    gridObjects.Add(cellPosition, machine.gameObject);
                }
            }
        }
    }

    // This method is called when the player presses the interact button (E)
    public void TryInteract(Vector3 indicatorWorldPosition)
    {
        Vector3Int indicatorCellPosition = grid.WorldToCell(indicatorWorldPosition);

        if (gridObjects.ContainsKey(indicatorCellPosition))
        {
            GameObject machineToPick = gridObjects[indicatorCellPosition];
            Debug.Log("Picked up: " + machineToPick.name);

            heldMachineName = machineToPick.name.Replace("(Clone)", "");  // Store the name of the machine
            Destroy(machineToPick);                     // Remove from the scene
            gridObjects.Remove(indicatorCellPosition);  // Remove from the dictionary
        }
        else
        {
            Debug.Log("No machine at this position.");
        }
    }

    // This method is called when the player presses the drop button (C)
    public void TryDrop(Vector3 indicatorWorldPosition)
    {
        if (!string.IsNullOrEmpty(heldMachineName))
        {
            Vector3Int dropCellPosition = grid.WorldToCell(indicatorWorldPosition);

            // Check if the grid position is empty
            if (!gridObjects.ContainsKey(dropCellPosition))
            {
                // Instantiate a new prefab using the stored machine name
                GameObject prefabToDrop = Resources.Load<GameObject>(heldMachineName);
                if (prefabToDrop != null)
                {
                    GameObject newMachine = Instantiate(prefabToDrop, grid.GetCellCenterWorld(dropCellPosition), Quaternion.identity, machinesContainer);
                    gridObjects.Add(dropCellPosition, newMachine);
                    Debug.Log("Dropped machine at: " + dropCellPosition);
                }
                else
                {
                    Debug.Log("Prefab not found in Resources folder: " + heldMachineName);
                }

                heldMachineName = "";  // Clear the held machine name after dropping
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
