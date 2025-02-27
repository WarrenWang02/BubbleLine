using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GridObjectsAsset gridObjectsAsset; 
    [SerializeField] private Grid grid; 
    [SerializeField] private Transform machinesContainer;
    [SerializeField] private Transform tutorial1Container; 
    [SerializeField] private Transform tutorial2Container; 
    [SerializeField] private Transform tutorial3Container;

    [SerializeField] private MachinePlacementChecker machinePlacementChecker;
    [SerializeField] private MachineRemovalChecker machineRemovalChecker;
    private void Start()
    {
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
            machineRemovalChecker.OnMachinesRemoved -= Tutorial3Trigger;
        }
    }

    public void Tutorial1Trigger()
    {
        EnableAndRegisterMachines(tutorial1Container);

        // Start machine placement detection after Tutorial 1 is triggered
        machinePlacementChecker.StartDetection();
    }

    public void Tutorial2Trigger()
    {
        EnableAndRegisterMachines(tutorial2Container);

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
        EnableAndRegisterMachines(tutorial3Container);
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
}
