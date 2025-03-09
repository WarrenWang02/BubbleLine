using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class IngredientDeadzone : MonoBehaviour
{
    [SerializeField] private OrderItem orderItemAsset; // Assign in Inspector

    // Event triggered when 3 milk ingredients are received
    public static event Action OnThreeMilksReceived;

    private int milkCount = 0;
    private bool isDetectingMilk = false; // Toggle for detection mode

    private void OnEnable()
    {
        EventManager.OnTutorial3Triggered += StartMilkDetection;
    }

    private void OnDisable()
    {
        EventManager.OnTutorial3Triggered -= StartMilkDetection;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            string ingredientName = GetNormalizedIngredientName(other.name);
            UpdateStoredAmount(ingredientName);
            Debug.Log($"Processed {ingredientName} in OrderItem.");

            if (isDetectingMilk && ingredientName == "milk")
            {
                milkCount++;
                if (milkCount >= 3)
                {
                    OnThreeMilksReceived?.Invoke();
                    Debug.Log("Three milk ingredients received! Event fired.");
                    
                    StopMilkDetection(); // Disable detection after firing event
                }
            }

            Destroy(other.gameObject);  // Remove the ingredient
        }
    }

    private string GetNormalizedIngredientName(string rawName)
    {
        // Convert to lowercase for consistency
        string normalized = rawName.ToLower();

        // Remove "(Clone)" if present
        normalized = Regex.Replace(normalized, @"\s*\(clone\)", "");

        // Remove any extra numbers in parentheses (like "(1)", "(2)")
        normalized = Regex.Replace(normalized, @"\s*\(\d+\)", "");

        // Trim extra spaces
        return normalized.Trim();
    }

    private void UpdateStoredAmount(string ingredientName)
    {
        if (orderItemAsset == null)
        {
            Debug.LogError("OrderItem asset is not assigned!");
            return;
        }

        foreach (Orders order in orderItemAsset.orders)
        {
            if (order.Product != null && GetNormalizedIngredientName(order.Product.name) == ingredientName)
            {
                order.RequireAmount -= 1;
                return;
            }
        }

        Debug.Log($"No matching product found for {ingredientName} in OrderItem.");
    }

    // Call this method to enable milk detection mode
    public void StartMilkDetection()
    {
        isDetectingMilk = true;
        milkCount = 0; // Reset count when detection starts
        Debug.Log("Milk detection started!");
    }

    // Call this method to disable milk detection after firing once
    private void StopMilkDetection()
    {
        isDetectingMilk = false;
        milkCount = 0; // Reset count after firing
        Debug.Log("Milk detection stopped.");
    }
}