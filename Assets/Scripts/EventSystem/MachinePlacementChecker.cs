using System;
using System.Collections.Generic;
using UnityEngine;

public class MachinePlacementChecker : MonoBehaviour
{
    public Grid grid;
    public GridObjectsAsset gridObjectsAsset;
    public Transform[] destinationIndicators;

    private List<Vector3Int> destinationPositions = new List<Vector3Int>();

    public event Action OnMachinesPlaced; // Event triggered when all 3 machines are placed

    private bool isDetectionActive = false;

    private void Start()
    {
        foreach (Transform indicator in destinationIndicators)
        {
            Vector3Int cellPosition = grid.WorldToCell(indicator.position);
            cellPosition.y = 0;
            destinationPositions.Add(cellPosition);
        }
    }

    public void StartDetection()
    {
        if (!isDetectionActive)
        {
            isDetectionActive = true;
            InvokeRepeating(nameof(CheckMachinePlacement), 0.5f, 0.5f);
        }
    }

    private void CheckMachinePlacement()
    {
        if (!isDetectionActive) return;

        int occupiedCount = 0;

        foreach (Vector3Int pos in destinationPositions)
        {
            if (gridObjectsAsset.gridObjects.ContainsKey(pos))
            {
                occupiedCount++;
            }
        }

        if (occupiedCount == 3)
        {
            isDetectionActive = false;
            CancelInvoke(nameof(CheckMachinePlacement));
            OnMachinesPlaced?.Invoke(); // Notify EventManager
        }
    }
}
