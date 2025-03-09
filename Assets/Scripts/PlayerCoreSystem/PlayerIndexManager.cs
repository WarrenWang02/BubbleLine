using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerIndexManager : MonoBehaviour
{
    private List<GameObject> trackedPlayers = new List<GameObject>();

    private void Update()
    {
        DetectNewPlayers();
    }

    private void DetectNewPlayers()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in allPlayers)
        {
            if (!trackedPlayers.Contains(player))
            {
                // New player detected, assign a number and update UI
                trackedPlayers.Add(player);
                int playerNumber = trackedPlayers.Count;

                UpdatePlayerNumber(player, playerNumber);
            }
        }
    }

    private void UpdatePlayerNumber(GameObject player, int number)
    {
        Transform canvasTransform = player.transform.Find("Canvas/PlayerNumber");

        if (canvasTransform != null)
        {
            TextMeshProUGUI playerNumberText = canvasTransform.GetComponent<TextMeshProUGUI>();

            if (playerNumberText != null)
            {
                playerNumberText.text = $"Player {number}";
                Debug.Log($"Updated {player.name} to Player {number}");
            }
            else
            {
                Debug.LogWarning($"Player {player.name} does not have a TextMeshProUGUI on Canvas/PlayerNumber.");
            }
        }
        else
        {
            Debug.LogWarning($"Player {player.name} does not have a Canvas/PlayerNumber child.");
        }
    }
}
