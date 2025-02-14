using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private Transform targetSpot;
    [SerializeField] private float targetSpeed = 2f;

    private void Start()
    {
        if (targetSpot == null)
        {
            Debug.LogError($"[ConveyorBelt] TargetSpot is missing on {gameObject.name}. Assign it in the Inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            IngredientMover ingredientMover = other.GetComponent<IngredientMover>();
            if (ingredientMover != null && targetSpot != null)
            {
                // Set target only if it hasn't already been set
                ingredientMover.SetTarget(targetSpot, targetSpeed);
            }
        }
    }
}
