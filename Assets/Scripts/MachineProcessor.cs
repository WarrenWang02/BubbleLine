using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections;

public class MachineProcessor : MonoBehaviour
{
    [SerializeField] private Transform outputPosition;
    [SerializeField] private Collider ingredientZone;
    [SerializeField] private RecipeData recipeData; // New scriptable object reference
    [SerializeField] private Image progressBar; // should be foreground image in the child
    [SerializeField] private float productionTime = 3f; // Editable countdown time in Inspector

    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
    private Coroutine productionCoroutine; // To handle the countdown process

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

        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
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

                //Debug.Log($"Added [{ingredientName}] Current count: {ingredientCounts[ingredientName]}"); //Debug for counting current ingredient

                CheckRecipe();
            }
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

    private void CheckRecipe()
    {
        if (recipeData == null)
        {
            Debug.LogError("No RecipeData assigned!");
            return;
        }

        RecipeData.Recipe matchingRecipe = recipeData.GetMatchingRecipe(ingredientCounts);

        if (matchingRecipe != null && HasEnoughIngredients(matchingRecipe))
        {
            // Start countdown instead of immediately producing output
            if (productionCoroutine == null)
            {
                productionCoroutine = StartCoroutine(ProductionCountdown(matchingRecipe));
            }
        }
    }
    private void ProduceOutput(RecipeData.Recipe recipe)
    {
        if (recipe.outputPrefab == null)
        {
            Debug.LogError($"No output prefab assigned for recipe: {recipe.recipeName}");
            return;
        }

        // Commented out block detection check
        /*
        if (Physics.CheckSphere(outputPosition.position, 0.5f))
        {
            Debug.Log("Output area is blocked, cannot produce a new item.");
            return;
        }
        */

        // Instantiate product at the output position
        GameObject newProduct = Instantiate(recipe.outputPrefab, outputPosition.position, Quaternion.identity);
        newProduct.name = recipe.recipeName; // Rename to match recipe

        // Find the "IngredientsContainer" GameObject in the scene
        GameObject container = GameObject.Find("IngredientsContainer");
        if (container != null)
        {
            newProduct.transform.SetParent(container.transform); // Attach to container
        }
        else
        {
            Debug.LogWarning("IngredientsContainer not found! Product spawned without a parent.");
        }

        Debug.Log($"Produced: {recipe.recipeName}");
    }

    private bool HasEnoughIngredients(RecipeData.Recipe recipe)
    {
        foreach (var requirement in recipe.ingredients)
        {
            if (!ingredientCounts.ContainsKey(requirement.ingredientName) || ingredientCounts[requirement.ingredientName] < requirement.requiredAmount)
            {
                return false;  // Not enough ingredients, so don't produce output
            }
        }
        return true; // All ingredients meet the required amount
    }

    private IEnumerator ProductionCountdown(RecipeData.Recipe matchingRecipe)
    {
        float elapsedTime = 0f;

        // Gradually fill UI bar over productionTime duration
        while (elapsedTime < productionTime)
        {
            elapsedTime += Time.deltaTime;
            if (progressBar != null)
            {
                progressBar.fillAmount = elapsedTime / productionTime;
            }
            yield return null;
        }

        // Ensure bar is full when complete
        if (progressBar != null)
        {
            progressBar.fillAmount = 1f;
        }

        // Produce output after countdown completes
        ProduceOutput(matchingRecipe);

        // Deduct ingredients
        foreach (var requirement in matchingRecipe.ingredients)
        {
            ingredientCounts[requirement.ingredientName] -= requirement.requiredAmount;
        }

        // Reset progress bar
        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
        }

        // Reset coroutine reference
        productionCoroutine = null;
    }
}
