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
}
