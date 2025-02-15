using Unity.Mathematics;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;                         // Unity's Grid component
    [SerializeField] private Transform playerIndicator;         // Reference to the player's facing direction
    [SerializeField] private GameObject indicatorPrefab;        // Visual frame for selected grid cell

    private GameObject currentIndicator; // Instance of the indicator prefab
    [SerializeField] private GameObject ghostPrefab;      // Reference to the ghost object (no instantiation)

    void Start()
    {
        // Instantiate the visual indicator at the start
        currentIndicator = Instantiate(indicatorPrefab);
    }

    void Update()
    {
        // Update the indicator's position to follow the player's facing direction
        UpdateIndicatorPosition();

        // Update ghost object position only if it exists
        if (ghostPrefab != null)
        {
            UpdateGhostPosition();
        }
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

    void UpdateGhostPosition()
    {
        if (ghostPrefab == null) return;

        // Get the indicator's current grid position
        Vector3Int cellPosition = grid.WorldToCell(playerIndicator.position);
        Vector3 ghostPosition = grid.GetCellCenterWorld(cellPosition);

        // Maintain the ghost object's original height
        ghostPosition.y = ghostPrefab.transform.position.y;

        // Instantly move the ghost object (no smoothing)
        ghostPrefab.transform.position = ghostPosition;
    }

    // Assign a GameObject to ghostPrefab
    public void SetGhostPrefab(GameObject prefab)
    {
        ghostPrefab = prefab;
    }

    // Clear the ghostPrefab reference
    public void DeleteGhostPrefab()
    {
        if (ghostPrefab != null)
        {
            Destroy(ghostPrefab); // Ensure the ghost prefab is removed from the scene
            ghostPrefab = null;
        }
    }
}