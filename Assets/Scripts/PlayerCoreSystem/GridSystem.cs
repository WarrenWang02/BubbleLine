using Unity.Mathematics;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;                         // Unity's Grid component
    [SerializeField] private Transform playerIndicator;         // Reference to the player's facing direction
    [SerializeField] private GameObject indicatorPrefab;        // Visual frame for selected grid cell
    private GroundManager groundManager;
    private int gridWidth;                     // Width of the gird system
    private int gridHeight;                    // Height of the gird system

    private GameObject currentIndicator; // Instance of the indicator prefab
    [SerializeField] private GameObject ghostPrefab;      // Reference to the ghost object (no instantiation)

    void Awake()
    {
        //Find ground in the scene and import width and height
        GameObject groundObj = GameObject.Find("Ground");
        if (groundObj != null)
        {
            groundManager = groundObj.GetComponent<GroundManager>();
            //Debug.Log("[DEBUG] Ground's manager assigned to groundManager.");
            gridWidth = groundManager.GetGridWidth();
            gridHeight = groundManager.GetGridHeight();
        }
        else
        {
            Debug.LogError("[ERROR] Ground not found in the scene!");
        }

        //Find Grid in the scene
        GameObject gridObj = GameObject.Find("Grid");
        if (gridObj != null)
        {
            grid = gridObj.GetComponent<Grid>();
            //Debug.Log("[DEBUG] Grid assigned to GridSystem.");
        }
        else
        {
            Debug.LogError("[ERROR] Grid not found in the scene!");
        }

        //Find Player Indicator (Child Object)
        Transform indicator = transform.Find("PlayerIndicator");
        if (indicator != null)
        {
            playerIndicator = indicator;
            Debug.Log("[DEBUG] Player Indicator assigned.");
        }
        else
        {
            Debug.LogError("[ERROR] Player Indicator not found inside Player prefab!");
        }
        
        // Instantiate the visual indicator at the Awake
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

        // Clamp the cell position within the grid bounds
        cellPosition.x = Mathf.Clamp(cellPosition.x, -gridWidth / 2, gridWidth / 2 - 1);
        cellPosition.z = Mathf.Clamp(cellPosition.z, -gridHeight / 2, gridHeight / 2 - 1); // if a 2D XZ grid

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

        // Clamp the cell position within the grid bounds
        cellPosition.x = Mathf.Clamp(cellPosition.x, -gridWidth / 2, gridWidth / 2 - 1);
        cellPosition.z = Mathf.Clamp(cellPosition.z, -gridHeight / 2, gridHeight / 2 - 1); // if a 2D XZ grid

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

    public void rotateGhostPrefab90(){
        if (ghostPrefab != null)
        {
            ghostPrefab.transform.rotation *= Quaternion.Euler(0, 90f, 0);
        }
    }
}