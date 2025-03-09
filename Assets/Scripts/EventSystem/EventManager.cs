using System;
using UnityEngine;

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

    public static event Action OnTutorial3Triggered; // Event for Tutorial3Trigger
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
    }

    private void OnDisable()
    {
        IngredientDeadzone.OnThreeMilksReceived -= Level1Trigger;
    }

    public void Tutorial1Trigger()
    {
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
        
        dialogueUIManager.StartDialogue(Tutorial4Dialog); // Dialogue3 start
        DisableAndDeregisterMachines(tutorial3Container);
        EnableAndRegisterMachines(level1Container);
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

        // Deactivate the container after removing machines from the grid
        if (Container.gameObject.activeSelf)
        {
            Container.gameObject.SetActive(false);
        }

        Debug.Log($"[DEBUG] Machines from '{Container.name}' disabled and deregistered.");
    }
}
