using System.Collections.Generic;
using UnityEngine;

public class IngredientDeadzone : MonoBehaviour
{
    private List<string> recordedIngredients = new List<string>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            string ingredientName = NormalizeIngredientName(other.name);
            recordedIngredients.Add(ingredientName);
            Debug.Log($"Recorded {ingredientName}. Total count: {recordedIngredients.Count}");

            Destroy(other.gameObject);  // Remove the ingredient
        }
    }

    private string NormalizeIngredientName(string rawName)
    {
        return rawName.ToLower().Trim();  // Simple normalization, modify as needed
    }

    public List<string> GetRecordedIngredients()
    {
        return new List<string>(recordedIngredients); // Return a copy to prevent modification
    }

    public void ClearRecordedIngredients()
    {
        recordedIngredients.Clear();
    }
}
