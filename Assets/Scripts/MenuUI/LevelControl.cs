using UnityEngine;
using TMPro;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private float LevelTime = 600f; // Default 10 minutes
    [SerializeField] private bool levelActive = true; // Main control for level status
    [SerializeField] private GameObject inGameUI; // Bind in the Inspector
    [SerializeField] private TextMeshProUGUI levelTimerText; // Bind TextMeshPro component

    private int lastDisplayedTime; // Stores last displayed time to prevent skipping

    private void Start()
    {
        if (levelActive)
        {
            StartLevel();
        }
    }

    private void Update()
    {
        if (!levelActive || LevelTime <= 0) return;

        // Countdown time every frame
        LevelTime -= Time.deltaTime;

        // Ensure level time never goes below 0
        if (LevelTime < 0) LevelTime = 0;

        // Update UI only when the integer second changes
        int currentDisplayedTime = Mathf.FloorToInt(LevelTime);
        if (currentDisplayedTime != lastDisplayedTime)
        {
            lastDisplayedTime = currentDisplayedTime;
            UpdateTimerText();
        }
    }

    public void StartLevel()
    {
        levelActive = true;
        if (inGameUI != null)
        {
            inGameUI.SetActive(true);
        }
    }

    public void StopLevel()
    {
        levelActive = false;
        if (inGameUI != null)
        {
            inGameUI.SetActive(false);
        }
    }

    private void UpdateTimerText()
    {
        if (levelTimerText != null)
        {
            int minutes = Mathf.FloorToInt(LevelTime / 60);
            int seconds = Mathf.FloorToInt(LevelTime % 60);
            levelTimerText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }

    public float GetLevelTime()
    {
        return LevelTime;
    }
}
