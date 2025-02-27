using UnityEngine;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    public GameObject dialoguePanel; // Assign the UI Panel in Inspector
    public TextMeshProUGUI dialogueText; // Assign the TMP Text object in Inspector

    private void Start()
    {
        dialoguePanel.SetActive(false); // Ensure it starts disabled
    }

    public void ToggleDialogue(bool isActive)
    {
        dialoguePanel.SetActive(isActive);
    }
}
