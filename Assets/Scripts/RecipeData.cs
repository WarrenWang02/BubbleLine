using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipeData", menuName = "GameSettings/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [System.Serializable]
    public class Recipe
    {
        public string productName; // Name of the final product
        public List<IngredientRequirement> ingredients; // Required ingredients
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
            bool matches = true;

            foreach (var requirement in recipe.ingredients)
            {
                if (!currentIngredients.ContainsKey(requirement.ingredientName) ||
                    currentIngredients[requirement.ingredientName] < requirement.requiredAmount)
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
                return recipe;
        }

        return null; // No matching recipe found
    }
}
