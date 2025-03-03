using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private float LevelTime = 600f; // Default 10 minutes
    [SerializeField] private bool levelActive = true; // Main control for level status
    [SerializeField] private GameObject inGameUI; // Bind in the Inspector
    [SerializeField] private TextMeshProUGUI levelTimerText; // Bind TextMeshPro component
    [SerializeField] private OrderItem currentOrders; // Bind manually in the Inspector
    [SerializeField] private GameObject orderPrefab; // Prefab to spawn
    [SerializeField] private GameObject orderCanvas; // Prefab to spawn

    private List<GameObject> spawnedOrders = new List<GameObject>();
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

        // Sync with currentOrders and spawn prefab if needed
        UpdateOrderDisplay();
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
            Vector3 spawnPos = orderPrefab.transform.position; // Default prefab position
            if (spawnedOrders.Count > 0)
            {
                float prefabWidth = orderPrefab.GetComponent<RectTransform>().sizeDelta.x;
                spawnPos.x += (prefabWidth + 10) * spawnedOrders.Count; // Offset next order
            }

            GameObject newOrder = Instantiate(orderPrefab, spawnPos, Quaternion.identity, orderCanvas.transform);
            spawnedOrders.Add(newOrder);
        }

        // Update child texts for each spawned order
        for (int i = 0; i < spawnedOrders.Count; i++)
        {
            if (i >= currentOrders.orders.Count) break; // Prevent errors if list changes

            Orders currentOrder = currentOrders.orders[i];
            GameObject orderObject = spawnedOrders[i];

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
                    earnText.text = $"{currentOrder.Price}";
                }
            }
        }
    }
}
