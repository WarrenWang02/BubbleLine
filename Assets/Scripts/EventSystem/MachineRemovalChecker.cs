using System;
using System.Collections.Generic;
using UnityEngine;

public class MachineRemovalChecker : MonoBehaviour
{
    public GridObjectsAsset gridObjectsAsset;
    public Grid grid;
    
    public List<Transform> trackedMachines = new List<Transform>(); // Machines added after Tutorial 2
    private HashSet<Transform> removedMachines = new HashSet<Transform>(); // Track which machines are already removed

    public event Action OnMachinesRemoved; // Event triggered when 5 tracked machines are deleted

    private bool isDetectionActive = false;

    public void StartDetection()
    {
        removedMachines.Clear(); // Reset tracking
        isDetectionActive = true;
        InvokeRepeating(nameof(CheckMachineRemoval), 0.5f, 0.5f);
    }

    private void CheckMachineRemoval()
    {
        if (!isDetectionActive) return;

        int removedCount = removedMachines.Count; // Start with already counted removals

        foreach (Transform machine in trackedMachines)
        {
            if (machine == null || !IsMachineInGrid(machine))
            {
                if (!removedMachines.Contains(machine)) // Only count new removals
                {
                    removedMachines.Add(machine);
                    removedCount++;
                }
            }
        }

        if (removedCount >= 5)
        {
            isDetectionActive = false;
            CancelInvoke(nameof(CheckMachineRemoval));
            OnMachinesRemoved?.Invoke(); // Notify EventManager
        }
    }

    private bool IsMachineInGrid(Transform machine)
    {
        Vector3Int cellPosition = grid.WorldToCell(machine.position);
        cellPosition.y = 0;
        return gridObjectsAsset.gridObjects.ContainsKey(cellPosition);
    }
}
