using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public Grid grid;                         // Unity's Grid component
    public Transform machinesContainer;       // Parent holding all machine objects

    private Dictionary<Vector3Int, GameObject> gridObjects = new Dictionary<Vector3Int, GameObject>();

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

            Destroy(machineToPick);                     // Remove from the scene
            gridObjects.Remove(indicatorCellPosition);  // Remove from the dictionary
        }
        else
        {
            Debug.Log("No machine at this position.");
        }
    }
}
