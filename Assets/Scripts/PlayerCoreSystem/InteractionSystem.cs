using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private GridObjectsAsset gridObjectsAsset; // Shared GridObjects dictionary
    [SerializeField] private Material testGhostMat;             // Manually assign in inspector
    [SerializeField] private DeletableMachinesList deletableMachinesList; // Manually assign in inspector
    [SerializeField] private TuneableMachinesList tuneableMachinesList; // Manually assign in inspector
    
    // SFX part
    [SerializeField] private AudioClip pickSFX;
    [SerializeField] private AudioClip dropSFX;
    [SerializeField] private AudioClip deleteSFX;
    [SerializeField] private AudioClip errorSFX;

    private AudioSource audioSource;

    private GameObject heldMachinePrefab = null; // Store the picked-up machine prefab
    private Dictionary<Vector3Int, GameObject> gridObjects; // Reference to asset dictionary
    private Grid grid;
    private Transform machinesContainer;
    private GridSystem gridSystem;
    private PlayerController playerController;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Dynamically find Grid
        GameObject gridObj = GameObject.Find("Grid");
        if (gridObj != null)
        {
            grid = gridObj.GetComponent<Grid>();
            Debug.Log("[DEBUG] Grid assigned.");
        }
        else
        {
            Debug.LogError("[ERROR] Grid not found in the scene!");
        }

        // Dynamically find MachinesContainer
        GameObject containerObj = GameObject.Find("MachinesContainer");
        if (containerObj != null)
        {
            machinesContainer = containerObj.transform;
            Debug.Log("[DEBUG] MachinesContainer assigned.");
        }
        else
        {
            Debug.LogError("[ERROR] MachinesContainer not found!");
        }

        // Get references on the same object
        gridSystem = GetComponent<GridSystem>();
        playerController = GetComponent<PlayerController>();

        if (gridObjectsAsset != null)
        {
            gridObjects = gridObjectsAsset.gridObjects;
        }
        else
        {
            Debug.LogError("[ERROR] GridObjectsAsset is missing!");
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
        gridSystem.SetGhostPrefab(ghostMachine);
    }

    public void DeSpawn()
    {
        heldMachinePrefab = null;
        gridSystem.DeleteGhostPrefab();
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
            PlaySound(pickSFX);
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
            gridSystem.SetGhostPrefab(ghostMachine);
        }
        else
        {
            PlaySound(errorSFX);
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

                GameObject newMachine = Instantiate(heldMachinePrefab, dropPosition, heldMachinePrefab.transform.rotation, heldMachinePrefab.transform.parent);
                newMachine.name = heldMachinePrefab.name;
                newMachine.SetActive(true);

                gridObjects.Add(dropCellPosition, newMachine);
                PlaySound(dropSFX);
                Debug.Log("Dropped machine at: " + dropCellPosition);

                //destroying original prefab in the field
                if (heldMachinePrefab.scene.IsValid()) 
                {
                    Destroy(heldMachinePrefab); // Only destroy if it exists in the scene
                }
                // Reset held machine and remove ghost preview
                heldMachinePrefab = null;
                gridSystem.DeleteGhostPrefab();
            }
            else
            {
                PlaySound(errorSFX);
                Debug.Log("Drop failed: Cell already occupied.");
            }
        }
        else
        {
            PlaySound(errorSFX);
            Debug.Log("No machine to drop.");
        }

        playerController.SpawnAgain();
    }

    private void ApplyGhostMaterial(GameObject targetObject, Material testGhostMat)
    {
        if (targetObject == null || testGhostMat == null) return;

        // First check for the main renderer
        Renderer targetRenderer = targetObject.GetComponent<Renderer>();
        if (targetRenderer != null && targetRenderer.sharedMaterial != null)
        {
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
                }
                else
                {
                    Debug.Log("not found ghostmat's base color");
                }
                if (newGhostMaterial.HasProperty("_Ghost_Color")) // Ensure GhostColor exists in shader
                {
                    newGhostMaterial.SetColor("_Ghost_Color", originalColor);
                }
                else
                {
                    Debug.Log("not found ghostmat's ghost color");
                }
            }

            // Assign the new ghost material to the target object
            targetRenderer.material = newGhostMaterial;
        }

        // Check for SpriteAttached child object
        Transform spriteAttachedTransform = targetObject.transform.Find("SpriteAttached");
        if (spriteAttachedTransform != null)
        {
            // Get all SpriteRenderer components under SpriteAttached
            SpriteRenderer[] spriteRenderers = spriteAttachedTransform.GetComponentsInChildren<SpriteRenderer>(true);
            
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer == null) continue;
                
                // Get current color and reduce alpha by half
                Color originalColor = spriteRenderer.color;
                Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a * 0.5f);
                
                // Apply the transparent color
                spriteRenderer.color = transparentColor;
            }
        }
    }
    
    public void RotateHeldMachine90()
    {
        if (heldMachinePrefab != null)
        {
            // Apply rotation around Y-axis
            heldMachinePrefab.transform.rotation *= Quaternion.Euler(0, 90f, 0);
            gridSystem.rotateGhostPrefab90();
            Debug.Log("Rotated held machine by 90 degrees.");
        }
    }

    public void TryDelete(Vector3 indicatorPosition)
    {
        Vector3Int cellPosition = grid.WorldToCell(indicatorPosition);
        cellPosition.y = 0;

        if (gridObjects.ContainsKey(cellPosition))
        {
            GameObject machineToDelete = gridObjects[cellPosition];

            if (deletableMachinesList.IsDeletable(machineToDelete.name))
            {
                Destroy(machineToDelete);
                gridObjects.Remove(cellPosition);
                PlaySound(deleteSFX);
                Debug.Log($"Deleted {machineToDelete.name} at {cellPosition}");
            }
            else if (tuneableMachinesList.IsTuneable(machineToDelete.name))
            {
                TryTuningMachine(machineToDelete);
            }
            else
            {
                PlaySound(errorSFX);
                Debug.Log($"Cannot delete/Tune {machineToDelete.name}, not in the lists.");
            }
        }
        else
        {
            PlaySound(errorSFX);
            Debug.Log("No machine at this position.");
        }
    }

    private void TryTuningMachine(GameObject machine)
    {
        machine.GetComponent<MachineProcessor>().SelectNextRecipe();
    }

    // Plays the given AudioClip
    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;  // Safety check

        if (audioSource.clip != clip)  // Only update if the clip is different
        {
            audioSource.clip = clip;  // Bind new clip
        }

        audioSource.Play();  // Play the sound
    }
}
