using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // Assign the pause UI Canvas in Inspector
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f; // Pause/unpause the game
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(isPaused); // Show/hide pause menu
        }
    }
}
