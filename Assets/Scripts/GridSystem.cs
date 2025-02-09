using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;                         // Unity's Grid component
    [SerializeField] private Transform playerIndicator;         // Reference to the player's facing direction (child object)
    [SerializeField] private GameObject indicatorPrefab;        // Visual frame that shows the selected grid cell

    private GameObject currentIndicator;      // Instance of the indicator prefab

    void Start()
    {
        // Instantiate the visual indicator at the start
        currentIndicator = Instantiate(indicatorPrefab);
    }

    void Update()
    {
        // Update the indicator's position to follow the player's facing direction
        UpdateIndicatorPosition();
    }

    void UpdateIndicatorPosition()
    {
        // Convert the player indicator's world position to the nearest grid cell
        Vector3Int cellPosition = grid.WorldToCell(playerIndicator.position);

        // Get the center of the grid cell in world space
        Vector3 snappedPosition = grid.GetCellCenterWorld(cellPosition);

        // Force Y position to 0 while keeping X and Z snapped to the grid
        snappedPosition.y = 0f;

        // Move the visual indicator to the adjusted grid position
        currentIndicator.transform.position = snappedPosition;
    }
}
