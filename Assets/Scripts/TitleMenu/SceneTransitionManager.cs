using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static List<PlayerConfig> RegisteredPlayers = new List<PlayerConfig>(); // Temporary storage for scene transition

    [System.Serializable]
    public class PlayerConfig
    {
        public int playerID;
        public string controlScheme;
    }

    [Header("Trigger Zones")]
    [SerializeField] private Collider startGameTriggerZone;  // Assign Start Game Trigger
    [SerializeField] private Collider exitTriggerZone;       // Assign Exit Trigger

    private void Update()
    {
        CheckStartGameZone();
        CheckRemovePlayerZone();
    }

    private void CheckStartGameZone()
    {
        // Get all active players
        PlayerInput[] activePlayers = FindObjectsOfType<PlayerInput>();

        if (activePlayers.Length == 0) return; // No players, do nothing

        bool allInStartZone = true;

        foreach (var player in activePlayers)
        {
            if (!IsPlayerInTrigger(player, startGameTriggerZone))
            {
                allInStartZone = false;
                break;
            }
        }

        if (allInStartZone)
        {
            Debug.Log("All players in Start Zone! Starting game...");
            TriggerSceneChange("Level1Scene");
        }
    }

    private void CheckRemovePlayerZone()
    {
        // Get all active players
        PlayerInput[] activePlayers = FindObjectsOfType<PlayerInput>();

        foreach (var player in activePlayers)
        {
            if (IsPlayerInTrigger(player, exitTriggerZone))
            {
                Debug.Log($"Player {player.playerIndex} exited! Removing from input system.");
                RemovePlayer(player);
            }
        }
    }

    private bool IsPlayerInTrigger(PlayerInput player, Collider triggerZone)
    {
        return triggerZone.bounds.Contains(player.transform.position);
    }

    private void RemovePlayer(PlayerInput player)
    {
        Destroy(player.gameObject); // Remove player object from the scene
        Debug.Log($"Player {player.playerIndex} removed.");
    }

    public void TriggerSceneChange(string sceneName)
    {
        // Clear previous data before registering new players
        RegisteredPlayers.Clear();

        // Find all active PlayerInput components in the scene
        PlayerInput[] activePlayers = FindObjectsOfType<PlayerInput>();

        foreach (PlayerInput player in activePlayers)
        {
            RegisteredPlayers.Add(new PlayerConfig
            {
                playerID = player.playerIndex,
                controlScheme = player.currentControlScheme
            });
        }

        // Debug log to check what is saved
        Debug.Log($"Transitioning with {RegisteredPlayers.Count} players.");

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }
}
