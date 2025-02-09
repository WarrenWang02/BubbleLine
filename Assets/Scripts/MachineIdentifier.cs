using UnityEngine;

public class MachineIdentifier : MonoBehaviour
{
    public GameObject originalPrefab;

    private void Awake()
    {
        if (originalPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing an originalPrefab reference!");
        }
    }
}