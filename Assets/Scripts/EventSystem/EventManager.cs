using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GridObjectsAsset gridObjectsAsset; 
    [SerializeField] private Grid grid; 
    [SerializeField] private Transform machinesContainer;
    [SerializeField] private Transform tutorial1Container; 
    [SerializeField] private Transform tutorial2Container; 
    [SerializeField] private Transform tutorial3Container;
    [SerializeField] private Transform level1Container;

    [SerializeField] private MachinePlacementChecker machinePlacementChecker;
    [SerializeField] private MachineRemovalChecker machineRemovalChecker;

    [SerializeField] private DialogueUIManager dialogueUIManager;
    [SerializeField] private DialogueData Tutorial1Dialog;
    [SerializeField] private DialogueData Tutorial2Dialog;
    [SerializeField] private DialogueData Tutorial3Dialog;
    [SerializeField] private DialogueData Tutorial4Dialog;
    [SerializeField] private LevelControl levelcontrol;
    [SerializeField] private GameObject TestUI;

    public static event Action OnTutorial3Triggered; // Event for Tutorial3Trigger

    private bool isWaitingForDialogue4End = false; // Flag to track when Dialogue 4 is running
    private void Start()
    {
        // This part is one time use like, not modular engough, if needed, make it better later.
        if (machinePlacementChecker != null)
        {
            // Subscribe to placement detection event
            machinePlacementChecker.OnMachinesPlaced += Tutorial2Trigger;
        }
        else
        {
            Debug.LogError("[ERROR] MachinePlacementChecker not found in the scene!");
        }

        if (machineRemovalChecker != null)
        {
            machineRemovalChecker.OnMachinesRemoved += Tutorial3Trigger;
        }
        else
        {
            Debug.LogError("[ERROR] MachineRemovalChecker not found in the scene!");
        }
    }

    private void OnDestroy()
    {
        if (machinePlacementChecker != null)
        {
            machinePlacementChecker.OnMachinesPlaced -= Tutorial2Trigger;
        }
        
        if (machinePlacementChecker != null)
        {
            machineRemovalChecker.OnMachinesRemoved -= Tutorial3Trigger;
        }  
    }

    private void OnEnable()
    {
        IngredientDeadzone.OnThreeMilksReceived += Level1Trigger;
        DialogueUIManager.OnDialogueEnding += HandleDialogueEnding;
    }

    private void OnDisable()
    {
        IngredientDeadzone.OnThreeMilksReceived -= Level1Trigger;
        DialogueUIManager.OnDialogueEnding -= HandleDialogueEnding;
    }

    public void Tutorial1Trigger()
    {
        TestUI.SetActive(false);
        EnableAndRegisterMachines(tutorial1Container);
        dialogueUIManager.StartDialogue(Tutorial1Dialog); // Dialogue1 start

        // Start machine placement detection after Tutorial 1 is triggered
        machinePlacementChecker.StartDetection();
    }

    public void Tutorial2Trigger()
    {
        DisableAndDeregisterMachines(tutorial1Container);
        EnableAndRegisterMachines(tutorial2Container);
        dialogueUIManager.StartDialogue(Tutorial2Dialog); // Dialogue2 start

        // Add tutorial2 machines to the removal checker
        foreach (Transform machine in tutorial2Container)
        {
            if (machine.CompareTag("machine"))
            {
                machineRemovalChecker.trackedMachines.Add(machine);
            }
        }

        // Start removal detection
        machineRemovalChecker.StartDetection();
    }

    public void Tutorial3Trigger()
    {
        DisableAndDeregisterMachines(tutorial2Container);
        EnableAndRegisterMachines(tutorial3Container);
        dialogueUIManager.StartDialogue(Tutorial3Dialog); // Dialogue3 start

        // Fire the event
        OnTutorial3Triggered?.Invoke();
        Debug.Log("Tutorial3Trigger event fired.");
    }

    public void Level1Trigger()
    {
        // Unsubscribe to prevent repeated triggers
        IngredientDeadzone.OnThreeMilksReceived -= Level1Trigger;

        // Start Dialogue 4 and wait for it to end before starting Level 1
        dialogueUIManager.StartDialogue(Tutorial4Dialog);
        isWaitingForDialogue4End = true;

        // Machines transition immediately, but Level 1 starts after dialogue ends
        DisableAndDeregisterMachines(tutorial3Container);
        EnableAndRegisterMachines(level1Container);
    }
    
    private void HandleDialogueEnding(DialogueData endedDialogue)
    {
        if (isWaitingForDialogue4End && endedDialogue == Tutorial4Dialog)
        {
            isWaitingForDialogue4End = false;
            Debug.Log("Dialogue 4 ended. Starting Level 1.");
            levelcontrol.StartLevel();
        }
    }

    private void EnableAndRegisterMachines(Transform Container)
    {
        if (Container == null)
        {
            Debug.LogWarning("[WARNING] Machine container not found.");
            return;
        }

        if (!Container.gameObject.activeSelf)
        {
            Container.gameObject.SetActive(true);
        }

        foreach (Transform machine in Container)
        {
            if (machine.CompareTag("machine"))
            {
                Vector3Int cellPosition = grid.WorldToCell(machine.position);
                cellPosition.y = 0;

                if (!gridObjectsAsset.gridObjects.ContainsKey(cellPosition))
                {
                    gridObjectsAsset.gridObjects.Add(cellPosition, machine.gameObject);
                }
            }
        }

        Debug.Log($"[DEBUG] Machines from '{Container.name}' enabled and registered.");
    }

    private void DisableAndDeregisterMachines(Transform Container)
    {
        if (Container == null)
        {
            Debug.LogWarning("[WARNING] Machine container not found.");
            return;
        }

        // Remove all machines from the specified container
        foreach (Transform machine in Container)
        {
            if (machine.CompareTag("machine"))
            {
                Vector3Int cellPosition = grid.WorldToCell(machine.position);
                cellPosition.y = 0;

                if (gridObjectsAsset.gridObjects.ContainsKey(cellPosition))
                {
                    gridObjectsAsset.gridObjects.Remove(cellPosition);
                }
            }
        }

        // Find all ConveyorBelts that are direct children of the scene root
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        List<GameObject> conveyorBeltsToRemove = new List<GameObject>();
        foreach (GameObject rootObject in rootObjects)
        {
            // Only process the object if it is a ConveyorBelt at the root level
            if (rootObject.name.Contains("ConveyorBelt"))
            {
                Vector3Int beltPosition = grid.WorldToCell(rootObject.transform.position);
                beltPosition.y = 0;

                if (gridObjectsAsset.gridObjects.ContainsKey(beltPosition))
                {
                    gridObjectsAsset.gridObjects.Remove(beltPosition);
                    conveyorBeltsToRemove.Add(rootObject);
                    Debug.Log($"[DEBUG] Root-level ConveyorBelt at {beltPosition} removed from dictionary.");
                }
            }
        }

        // Destroy ConveyorBelt objects after iterating to avoid modifying the list during iteration
        foreach (GameObject conveyorBelt in conveyorBeltsToRemove)
        {
            Destroy(conveyorBelt);
            Debug.Log($"[DEBUG] ConveyorBelt '{conveyorBelt.name}' destroyed.");
        }

        // Deactivate the container after removing machines from the grid
        if (Container.gameObject.activeSelf)
        {
            Container.gameObject.SetActive(false);
        }

        Debug.Log($"[DEBUG] Machines from '{Container.name}' disabled and deregistered.");
    }
}
