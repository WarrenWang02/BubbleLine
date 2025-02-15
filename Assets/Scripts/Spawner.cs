using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn; // Prefab assigned in Inspector or by script
    [SerializeField] private float spawnRadius = 0.3f; // Radius to check for obstacles
    [SerializeField] private float spawnInterval = 2f; // Time in seconds between spawns (adjustable)
    
    private float spawnTimer = 0f; // Timer to track spawn intervals

    void Update()
    {
        spawnTimer += Time.deltaTime; // Increase timer every frame

        if (spawnTimer >= spawnInterval) // If enough time has passed, spawn
        {
            Spawn();
            spawnTimer = 0f; // Reset timer after spawning
        }
    }
    public void Spawn()
    {
        // Check if the spawn area is clear
        if (IsSpawnLocationBlocked())
        {
            //Debug.LogWarning("Spawn blocked! Cannot spawn " + prefabToSpawn.name + " at " + transform.position);
            return; // Prevent spawning
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            Debug.Log("Spawned: " + prefabToSpawn.name + " at " + transform.position);
        }
        else
        {
            Debug.LogError("Spawner has no prefab assigned!");
        }
    }

    // Check if something is blocking the spawn point
    private bool IsSpawnLocationBlocked()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, spawnRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag(prefabToSpawn.tag)) // Only block if it's the same type
            {
                return true;
            }
        }

        return false; // No matching objects, spawn is allowed
    }

    // Optional: Allow setting prefab by script
    public void SetPrefab(GameObject newPrefab)
    {
        prefabToSpawn = newPrefab;
    }
}
