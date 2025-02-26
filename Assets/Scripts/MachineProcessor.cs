using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Linq;

public class MachineProcessor : MonoBehaviour
{
    [SerializeField] private Transform outputPosition;
    [SerializeField] private Collider ingredientZone;
    [SerializeField] private RecipeData recipeData; // New scriptable object reference
    [SerializeField] private Image progressBar; // should be foreground image in the child
    [SerializeField] private float productionTime = 3f; // Editable countdown time in Inspector
    [SerializeField] private int selectedRecipeIndex = 0;
    [SerializeField] private TextMeshProUGUI ingredientTextUI; // UI Text for ingredient display

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

        UpdateIngredientUI(); // Initialize UI text
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            string ingredientName = GetNormalizedIngredientName(other.name);
            RecipeData.Recipe selectedRecipe = recipeData.recipes[selectedRecipeIndex];

            if (selectedRecipe.ingredients.Exists(i => i.ingredientName == ingredientName))
            {
                // Only record the ingredient if it's in the selected recipe
                if (!ingredientCounts.ContainsKey(ingredientName))
                {
                    ingredientCounts[ingredientName] = 0;
                }
                ingredientCounts[ingredientName]++;
                
                UpdateIngredientUI(); // Update UI with new ingredient count
            }
            else
            {
                Debug.LogWarning($"[{ingredientName}] is not in the recipe and has been destroyed.");
            }

            Destroy(other.gameObject); // Always destroy the ingredient
            CheckRecipe();
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
        if (recipeData == null || recipeData.recipes.Count == 0)
        {
            Debug.Log("No RecipeData assigned or no recipes available!");
            return;
        }

        if (selectedRecipeIndex < 0 || selectedRecipeIndex >= recipeData.recipes.Count)
        {
            Debug.Log("Selected recipe index is out of range!");
            return;
        }

        RecipeData.Recipe selectedRecipe = recipeData.recipes[selectedRecipeIndex];

        if (HasEnoughIngredients(selectedRecipe))
        {
            if (productionCoroutine == null)
            {
                productionCoroutine = StartCoroutine(ProductionCountdown(selectedRecipe));
            }
        }
    }

    private void UpdateIngredientUI()
    {
        if (ingredientTextUI == null || recipeData == null || recipeData.recipes.Count == 0)
            return;

        RecipeData.Recipe selectedRecipe = recipeData.recipes[selectedRecipeIndex];

        // Update RecipeSprite's sprite based on the current recipe
        UpdateRecipeSprite(selectedRecipe);

        // Convert recipe name to uppercase for the first line
        // string displayText = $"<size=28><b>Producing: {selectedRecipe.recipeName.ToUpper()}</b></size>\n";
        string displayText = $"";

        foreach (var requirement in selectedRecipe.ingredients)
        {
            string ingredientName = char.ToUpper(requirement.ingredientName[0]) + requirement.ingredientName.Substring(1);
            int currentAmount = ingredientCounts.ContainsKey(requirement.ingredientName) ? ingredientCounts[requirement.ingredientName] : 0;

            // Color logic: Green if sufficient, Red if not enough
            string color = (currentAmount >= requirement.requiredAmount) ? "green" : "red";

            // Format: Ingredient Name: <color=red/green>Amount</color>
            displayText += $"{ingredientName}: <color={color}>{currentAmount}</color>  ";
        }

        // Assign formatted text to UI
        ingredientTextUI.text = displayText.TrimEnd(); // Trim to remove the last newline
    }

    private void UpdateRecipeSprite(RecipeData.Recipe recipe)
    {
        if (recipe.outputPrefab == null)
        {
            Debug.LogWarning($"No output prefab assigned for recipe: {recipe.recipeName}");
            return;
        }

        // Find "RecipeSprite" child object
        Transform recipeSpriteTransform = transform.Find("RecipeSprite");
        if (recipeSpriteTransform == null)
        {
            Debug.LogWarning("RecipeSprite child object not found!");
            return;
        }

        SpriteRenderer recipeSpriteRenderer = recipeSpriteTransform.GetComponent<SpriteRenderer>();
        if (recipeSpriteRenderer == null)
        {
            Debug.LogWarning("RecipeSprite object does not have a SpriteRenderer!");
            return;
        }

        // Get the sprite from the recipe's output prefab
        SpriteRenderer prefabSpriteRenderer = recipe.outputPrefab.GetComponent<SpriteRenderer>();
        if (prefabSpriteRenderer != null)
        {
            recipeSpriteRenderer.sprite = prefabSpriteRenderer.sprite; // Assign sprite
        }
        else
        {
            Debug.LogWarning($"Output prefab '{recipe.outputPrefab.name}' does not have a SpriteRenderer!");
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
    public void SelectNextRecipe()
    {
        if (recipeData == null || recipeData.recipes.Count == 0)
        {
            Debug.LogWarning("No recipes available in RecipeData!");
            return;
        }

        // Move to the next index, loop back to 0 if at the end
        selectedRecipeIndex = (selectedRecipeIndex + 1) % recipeData.recipes.Count;

        Debug.Log($"Selected recipe changed to: {recipeData.recipes[selectedRecipeIndex].recipeName}");
    }
}
