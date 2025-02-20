using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "TuneableMachinesList", menuName = "GameSettings/TuneableMachinesList")]
public class TuneableMachinesList : ScriptableObject
{
    [SerializeField] private List<string> TuneableMachineNames = new List<string> { "conveyorbelt" };

    public bool IsTuneable(string machineName)
    {
        string normalized = NormalizeMachineName(machineName);
        foreach (string name in TuneableMachineNames)
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