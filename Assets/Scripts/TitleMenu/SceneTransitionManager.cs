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

    [Header("Visual Feedback - Start Game Zone")]
    [SerializeField] private SpriteRenderer startGameSprite; // Assign sprite for start zone
    [SerializeField] private Color startGameTargetColor = Color.green; // Color when ready
    private Color startGameOriginalColor; // Store original color

    [Header("Visual Feedback - Remove Player Zone")]
    [SerializeField] private SpriteRenderer removePlayerSprite; // Assign sprite for remove zone
    [SerializeField] private Color removePlayerTargetColor = Color.red; // Color when removing player
    private Color removePlayerOriginalColor; // Store original color

    [Header("Settings")]
    [SerializeField] private float checkDuration = 3f; // Adjustable waiting time

    private float startGameTimer = 0f;
    private float removePlayerTimer = 0f;

    private void Start()
    {
        if (startGameSprite != null)
        {
            startGameOriginalColor = startGameSprite.color; // Store original color at start
        }

        if (removePlayerSprite != null)
        {
            removePlayerOriginalColor = removePlayerSprite.color; // Store original color at start
        }
    }

    private void Update()
    {
        CheckStartGameZone();
        CheckRemovePlayerZone();
        UpdateSpriteColors();
    }

    private void CheckStartGameZone()
    {
        PlayerInput[] activePlayers = FindObjectsOfType<PlayerInput>();

        if (activePlayers.Length == 0)
        {
            startGameTimer = 0f; // Reset timer if no players
            return;
        }

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
            startGameTimer += Time.deltaTime;

            if (startGameTimer >= checkDuration)
            {
                Debug.Log("All players in Start Zone for " + checkDuration + " seconds! Starting game...");
                TriggerSceneChange("Level1Scene");
            }
        }
        else
        {
            startGameTimer = 0f; // Reset if condition fails
        }
    }

    private void CheckRemovePlayerZone()
    {
        PlayerInput[] activePlayers = FindObjectsOfType<PlayerInput>();

        foreach (var player in activePlayers)
        {
            if (IsPlayerInTrigger(player, exitTriggerZone))
            {
                removePlayerTimer += Time.deltaTime;

                if (removePlayerTimer >= checkDuration)
                {
                    Debug.Log($"Player {player.playerIndex} stayed in Exit Zone for {checkDuration} seconds. Removing.");
                    RemovePlayer(player);
                }
            }
            else
            {
                removePlayerTimer = 0f; // Reset if player leaves
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

    private void UpdateSpriteColors()
    {
        if (startGameSprite != null)
        {
            float startProgress = Mathf.Clamp01(startGameTimer / checkDuration);
            startGameSprite.color = Color.Lerp(startGameOriginalColor, startGameTargetColor, startProgress);
        }

        if (removePlayerSprite != null)
        {
            float removeProgress = Mathf.Clamp01(removePlayerTimer / checkDuration);
            removePlayerSprite.color = Color.Lerp(removePlayerOriginalColor, removePlayerTargetColor, removeProgress);
        }
    }

    public void TriggerSceneChange(string sceneName)
    {
        RegisteredPlayers.Clear();

        PlayerInput[] activePlayers = FindObjectsOfType<PlayerInput>();

        foreach (PlayerInput player in activePlayers)
        {
            RegisteredPlayers.Add(new PlayerConfig
            {
                playerID = player.playerIndex,
                controlScheme = player.currentControlScheme
            });
        }

        Debug.Log($"Transitioning with {RegisteredPlayers.Count} players.");
        SceneManager.LoadScene(sceneName);
    }
}
