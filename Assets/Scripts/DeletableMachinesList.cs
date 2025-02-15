using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "DeletableMachinesList", menuName = "GameSettings/DeletableMachinesList")]
public class DeletableMachinesList : ScriptableObject
{
    [SerializeField] private List<string> deletableMachineNames = new List<string> { "conveyorbelt" };

    public bool IsDeletable(string machineName)
    {
        string normalized = NormalizeMachineName(machineName);
        foreach (string name in deletableMachineNames)
        {
            if (normalized == name) return true;
        }
        return false;
    }

    private string NormalizeMachineName(string rawName)
    {
        return Regex.Replace(rawName.ToLower(), @"\s*\(\d+\)", "").Trim(); // Remove clone numbers like (1), (2)
    }
}