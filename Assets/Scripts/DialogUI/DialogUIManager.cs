using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUIManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel; // Assign the UI Panel in Inspector
    [SerializeField] private TextMeshProUGUI dialogueText; // Assign the TMP Text object in Inspector

    private void Start()
    {
        dialoguePanel.SetActive(false); // Ensure it starts disabled
    }

    private void ToggleDialogue(bool isActive)
    {
        dialoguePanel.SetActive(isActive);
    }

        public void SetAllPlayersInputMap(string inputMapName)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap(inputMapName);
            }
        }
    }
}
