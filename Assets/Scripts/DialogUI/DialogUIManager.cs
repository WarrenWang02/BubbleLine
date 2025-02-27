using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUIManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel; // Assign the UI Panel in Inspector
    [SerializeField] private TextMeshProUGUI dialogueText; // Assign the TMP Text object in Inspector

    private DialogueData currentDialogue;
    private int dialogueIndex;
    private bool isDialogueActive = false;

    private void Start()
    {
        dialoguePanel.SetActive(false); // Ensure it starts disabled
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        if (dialogueData == null || dialogueData.dialogueEntries.Count == 0)
        {
            Debug.LogWarning("Invalid dialogue data or empty dialogue list.");
            return;
        }

        currentDialogue = dialogueData;
        dialogueIndex = 0;
        isDialogueActive = true;
        ToggleDialogue(true);
        ShowCurrentDialogue();
        SetAllPlayersInputMap("Dialog");
    }

    public void MoveNextDialogue()
    {
        if (!isDialogueActive) return;

        dialogueIndex++;
        if (dialogueIndex >= currentDialogue.dialogueEntries.Count)
        {
            EndDialogue();
            return;
        }

        ShowCurrentDialogue();
    }

    private void ShowCurrentDialogue()
    {
        if (currentDialogue != null && dialogueIndex < currentDialogue.dialogueEntries.Count)
        {
            dialogueText.text = currentDialogue.dialogueEntries[dialogueIndex];
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        ToggleDialogue(false);
        currentDialogue = null;
        SetAllPlayersInputMap("Player");
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
