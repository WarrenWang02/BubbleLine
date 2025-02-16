using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipeData", menuName = "GameSettings/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [System.Serializable]
    public class Recipe
    {
        public string recipeName; // Name for reference in the Inspector
        public List<IngredientRequirement> ingredients; // Required ingredients
        public GameObject outputPrefab; // Assignable prefab for output in the Inspector
    }

    [System.Serializable]
    public class IngredientRequirement
    {
        public string ingredientName;
        public int requiredAmount;
    }

    public List<Recipe> recipes = new List<Recipe>();

    public Recipe GetMatchingRecipe(Dictionary<string, int> currentIngredients)
    {
        foreach (var recipe in recipes)
        {
            if (HasEnoughIngredients(recipe, currentIngredients))
                return recipe; // Now we return the full recipe, including the prefab
        }
        return null;
    }

    private bool HasEnoughIngredients(Recipe recipe, Dictionary<string, int> currentIngredients)
    {
        foreach (var requirement in recipe.ingredients)
        {
            if (!currentIngredients.ContainsKey(requirement.ingredientName) ||
                currentIngredients[requirement.ingredientName] < requirement.requiredAmount)
            {
                return false;
            }
        }
        return true;
    }
}
