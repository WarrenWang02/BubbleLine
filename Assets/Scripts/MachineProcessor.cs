using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class MachineProcessor : MonoBehaviour
{
    [SerializeField] private Transform outputPosition;
    [SerializeField] private Collider ingredientZone;
    [SerializeField] private RecipeData recipeData; // New scriptable object reference

    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();

    private void Start()
    {
        if (ingredientZone != null)
        {
            ingredientZone.isTrigger = true;
        }
        else
        {
            Debug.LogError("Ingredient Zone is not assigned in the Inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            string ingredientName = GetNormalizedIngredientName(other.name);
            Destroy(other.gameObject); // Remove ingredient

            if (!string.IsNullOrEmpty(ingredientName))
            {
                if (!ingredientCounts.ContainsKey(ingredientName))
                {
                    ingredientCounts[ingredientName] = 0;
                }
                ingredientCounts[ingredientName]++;

                Debug.Log($"Added {ingredientName}. Current count: {ingredientCounts[ingredientName]}");

                CheckRecipe();
            }
        }
    }

    private string GetNormalizedIngredientName(string rawName)
    {
        return Regex.Replace(rawName.ToLower(), @"\s*\(\d+\)", "").Trim();
    }

    private void CheckRecipe()
    {
        if (recipeData == null)
        {
            Debug.LogError("No RecipeData assigned!");
            return;
        }

        RecipeData.Recipe matchingRecipe = recipeData.GetMatchingRecipe(ingredientCounts);

        if (matchingRecipe != null)
        {
            ProduceOutput(matchingRecipe.productName);

            foreach (var requirement in matchingRecipe.ingredients)
            {
                ingredientCounts[requirement.ingredientName] -= requirement.requiredAmount;
            }
        }
    }

    private void ProduceOutput(string productName)
    {
        if (Physics.CheckSphere(outputPosition.position, 0.5f))
        {
            Debug.Log("Output area is blocked, cannot produce a new item.");
            return;
        }

        GameObject newProduct = Instantiate(Resources.Load<GameObject>(productName), outputPosition.position, Quaternion.identity);
        Debug.Log($"Produced: {productName}");
    }
}
