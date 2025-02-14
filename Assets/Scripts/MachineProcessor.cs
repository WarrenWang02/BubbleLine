using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MachineProcessor : MonoBehaviour
{
    [SerializeField] private Transform outputPosition;  // Output spawn location
    [SerializeField] private Collider ingredientZone;   // Collider zone to detect ingredients

    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
    private HashSet<string> allowedIngredients = new HashSet<string> { "apple", "banana" };

    private void Start()
    {
        if (ingredientZone != null)
        {
            ingredientZone.isTrigger = true;  // Ensure the collider is set to trigger
        }
        else
        {
            Debug.LogError("Ingredient Zone is not assigned in the Inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ensure the trigger was caused by an ingredient entering the ingredientZone
        if (other.CompareTag("Ingredient"))
        {
            string ingredientName = GetNormalizedIngredientName(other.name);
            Destroy(other.gameObject);  // Destroy the ingredient when it enters the zone

            // Only record allowed ingredients
            if (allowedIngredients.Contains(ingredientName))
            {
                if (!ingredientCounts.ContainsKey(ingredientName))
                {
                    ingredientCounts[ingredientName] = 0;
                }
                ingredientCounts[ingredientName]++;
                Debug.Log($"Added {ingredientName}. Current count: {ingredientCounts[ingredientName]}");

                CheckRecipe();
            }
            else
            {
                Debug.Log($"Ignored {other.name}, not part of the recipe.");
            }
        }
    }

    private string GetNormalizedIngredientName(string rawName)
    {
        if (Regex.IsMatch(rawName, @"\bapple\b", RegexOptions.IgnoreCase))
        {
            return "apple";
        }
        if (Regex.IsMatch(rawName, @"\bbanana\b", RegexOptions.IgnoreCase))
        {
            return "banana";
        }
        return "";
    }

    private void CheckRecipe()
    {
        if (ingredientCounts.ContainsKey("apple") && ingredientCounts["apple"] >= 2 &&
            ingredientCounts.ContainsKey("banana") && ingredientCounts["banana"] >= 1)
        {
            ProduceOutput();
            ingredientCounts["apple"] -= 2;
            ingredientCounts["banana"] -= 1;
        }
    }

    private void ProduceOutput()
    {
        if (Physics.CheckSphere(outputPosition.position, 0.5f))
        {
            Debug.Log("Output area is blocked, cannot produce a new item.");
            return;
        }

        GameObject newProduct = Instantiate(Resources.Load<GameObject>("ProductPrefab"), outputPosition.position, Quaternion.identity);
        Debug.Log("Produced a new product!");
    }
}
