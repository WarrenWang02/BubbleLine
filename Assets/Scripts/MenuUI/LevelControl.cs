using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private float LevelTime = 600f; // Default 10 minutes
    [SerializeField] private bool levelActive = false; // Main control for level status
    [SerializeField] private GameObject inGameUI; // Bind in the Inspector
    [SerializeField] private TextMeshProUGUI levelTimerText; // Bind TextMeshPro component
    [SerializeField] private OrderItem currentOrders; // Bind manually in the Inspector
    [SerializeField] private GameObject orderPrefab; // Prefab to spawn
    [SerializeField] private GameObject orderCanvas; // Prefab to spawn
    [SerializeField] private TextMeshProUGUI earnedText; // UI Text to show earned amount
    [SerializeField] private int rankingTier1 = 100; // First tier threshold
    [SerializeField] private int rankingTier2 = 200; // Second tier threshold
    [SerializeField] private int rankingTier3 = 300; // Third tier threshold
    [SerializeField] private Image tier1Image; // UI Image for Tier 1
    [SerializeField] private Image tier2Image; // UI Image for Tier 2
    [SerializeField] private Image tier3Image; // UI Image for Tier 3


    private List<GameObject> spawnedOrders = new List<GameObject>();
    private int lastDisplayedTime; // Stores last displayed time to prevent skipping
    private int earned = 0; // Track total earned money
    private Dictionary<Orders, float> initialOrderTimes = new Dictionary<Orders, float>(); // Store initial times

    private void Start()
    {
        levelActive = false;
        UpdateEarnedUI();
        UpdateRankingUI();
    }

    private void Update()
    {
        if (!levelActive || LevelTime <= 0) return;

        if (levelActive) 
        {
            // Countdown time every frame
            LevelTime -= Time.deltaTime;
        }

        // Ensure level time never goes below 0
        if (LevelTime < 0) LevelTime = 0;

        // Update UI only when the integer second changes
        int currentDisplayedTime = Mathf.FloorToInt(LevelTime);
        if (currentDisplayedTime != lastDisplayedTime)
        {
            lastDisplayedTime = currentDisplayedTime;
            UpdateTimerText();
        }

        // Sync with currentOrders and spawn prefab if needed
        UpdateOrderDisplay();

        // Continuously check for expired or completed orders
        RemoveCompletedOrders();
    }

    public void StartLevel()
    {
        levelActive = true;
        if (inGameUI != null)
        {
            inGameUI.SetActive(true);
        }

        // Start the order countdown
        StartCoroutine(UpdateOrderTimers());
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

    private IEnumerator UpdateOrderTimers()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Reduce time every 1 second
            foreach (var order in currentOrders.orders)
            {
                order.Time = Mathf.Max(0, order.Time - 1); // Decrease time, but ensure it doesn’t go below 0
            }
            RemoveCompletedOrders(); // Check after reducing time
        }
    }

    private void RemoveCompletedOrders()
    {
        for (int i = currentOrders.orders.Count - 1; i >= 0; i--) // Loop in reverse to safely remove
        {
            Orders order = currentOrders.orders[i];

            if (order.RequireAmount <= 0)
            {
                earned += order.Price; // Add price to earned
                Debug.Log($"Order completed: {order.Product.name}, Earned: {order.Price}, Total Earned: {earned}");
                RemoveOrderFromUI(i);
                currentOrders.orders.RemoveAt(i);

                // Update Earned Display & Ranking UI
                UpdateEarnedUI();
                UpdateRankingUI();
            }
            else if (order.Time <= 0)
            {
                Debug.Log($"Order expired: {order.Product.name}, No earnings.");
                RemoveOrderFromUI(i);
                currentOrders.orders.RemoveAt(i);
            }
        }
    }

    private void RemoveOrderFromUI(int index)
    {
        if (index < spawnedOrders.Count)
        {
            Destroy(spawnedOrders[index]); // Remove UI element
            spawnedOrders.RemoveAt(index);
        }

        // Immediately reposition remaining orders
        UpdateOrderDisplay();
    }

    private void UpdateOrderDisplay()
    {
        // Clear existing orders if the list is empty
        if (currentOrders == null || currentOrders.orders.Count == 0)
        {
            foreach (GameObject obj in spawnedOrders)
            {
                Destroy(obj);
            }
            spawnedOrders.Clear();
            return;
        }

        // Adjust the number of spawned prefabs to match the number of orders
        while (spawnedOrders.Count < currentOrders.orders.Count)
        {
            GameObject newOrder = Instantiate(orderPrefab, orderCanvas.transform);
            spawnedOrders.Add(newOrder);
        }

        // Get the original prefab's position (only use for the first order)
        Vector2 startPosition = orderPrefab.GetComponent<RectTransform>().anchoredPosition;

        // Correctly position orders based on their **index**
        for (int i = 0; i < spawnedOrders.Count; i++)
        {
            if (i >= currentOrders.orders.Count) break; // Prevent errors if list changes

            GameObject orderObject = spawnedOrders[i];
            Orders currentOrder = currentOrders.orders[i];

            // Store the Initial Time if not already recorded
            if (!initialOrderTimes.ContainsKey(currentOrder))
            {
                initialOrderTimes[currentOrder] = currentOrder.Time; // Save initial time
            }

            float initialTime = initialOrderTimes[currentOrder]; // Get the starting time

            // Use the original prefab's position as the base for the first order
            RectTransform rectTransform = orderObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float yOffset = startPosition.y - (i * (rectTransform.sizeDelta.y * rectTransform.localScale.y + 10)); // Stack orders downward
                rectTransform.anchoredPosition = new Vector2(startPosition.x, yOffset);
            }

            // Update text
            Transform requirementChild = orderObject.transform.Find("Requirement");
            Transform earnChild = orderObject.transform.Find("Earn");

            if (requirementChild != null)
            {
                TextMeshProUGUI requirementText = requirementChild.GetComponent<TextMeshProUGUI>();
                if (requirementText != null)
                {
                    requirementText.text = $"{currentOrder.Product.name}: x {currentOrder.RequireAmount}";
                }
            }

            if (earnChild != null)
            {
                TextMeshProUGUI earnText = earnChild.GetComponent<TextMeshProUGUI>();
                if (earnText != null)
                {
                    earnText.text = $"Profit: {currentOrder.Price}";
                }
            }

            // Update Foreground Fill Amount & Color Transition
            Transform foregroundTransform = orderObject.transform.Find("Background/Foreground");
            if (foregroundTransform != null)
            {
                UnityEngine.UI.Image foregroundImage = foregroundTransform.GetComponent<UnityEngine.UI.Image>();
                if (foregroundImage != null)
                {
                    // Calculate Fill Amount using the Initial Time
                    float fillAmount = Mathf.Clamp01(currentOrder.Time / initialTime); // Normalize 0-1
                    foregroundImage.fillAmount = fillAmount;

                    // Smoothly Transition Color (Green → Yellow → Red)
                    foregroundImage.color = Color.Lerp(Color.red, Color.green, fillAmount);
                }
            }
        }
    }

    public void RemoveFirstOrder()
    {
        if (currentOrders != null && currentOrders.orders.Count > 0)
        {
            // Remove the first order from the list
            currentOrders.orders.RemoveAt(0);

            // Destroy the first spawned order UI element if it exists
            if (spawnedOrders.Count > 0)
            {
                Destroy(spawnedOrders[0]);
                spawnedOrders.RemoveAt(0);
            }
        }
    }

    private void UpdateEarnedUI()
    {
        if (earnedText != null)
        {
            earnedText.text = $"Earned: {earned}";
        }
    }

    private void UpdateRankingUI()
    {
        Color originalColor = new Color(178f / 255f, 178f / 255f, 178f / 255f); // RGB(178, 178, 178) converted to Unity Color

        // ✅ Change tier images to Yellow if earned value meets conditions, else set to original gray color
        if (tier1Image != null)
            tier1Image.color = (earned >= rankingTier1) ? Color.yellow : originalColor;

        if (tier2Image != null)
            tier2Image.color = (earned >= rankingTier2) ? Color.yellow : originalColor;

        if (tier3Image != null)
            tier3Image.color = (earned >= rankingTier3) ? Color.yellow : originalColor;
    }
}
