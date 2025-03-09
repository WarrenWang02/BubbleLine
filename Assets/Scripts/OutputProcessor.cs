using System.Collections.Generic;
using UnityEngine;

public class IngredientDeadzone : MonoBehaviour
{
    [SerializeField] private OrderItem orderItemAsset; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            string ingredientName = NormalizeIngredientName(other.name);
            UpdateStoredAmount(ingredientName);
            Debug.Log($"Processed {ingredientName} in OrderItem.");

            Destroy(other.gameObject);  // Remove the ingredient
        }
    }

    private string NormalizeIngredientName(string rawName)
    {
        return rawName.ToLower().Trim();  // Simple normalization, modify as needed
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
            if (order.Product != null && NormalizeIngredientName(order.Product.name) == ingredientName)
            {
                order.RequireAmount -= 1;
                //Debug.Log($"Updated {order.Product.name}: StoredAmount is now {order.StoredAmount}");
                return;
            }
        }

        Debug.Log($"No matching product found for {ingredientName} in OrderItem.");
    }
}
