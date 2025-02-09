using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Machine
{
    public string machineName;  // Name identifier for the machine
    public GameObject prefab;   // Reference to the prefab
}

public class MachineList : MonoBehaviour
{
    [SerializeField] private List<Machine> machinePrefabs;      // List to store machine prefab references

    // Method to find a prefab by machine name
    public GameObject GetPrefabByName(string machineName)
    {
        Machine matchingMachine = machinePrefabs.Find(m => m.machineName == machineName);
        return matchingMachine != null ? matchingMachine.prefab : null;
    }
}
