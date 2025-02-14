using UnityEngine;

public class IngredientMover : MonoBehaviour
{
    [SerializeField] private Transform targetSpot; // Target position
    [SerializeField] private float moveSpeed = 2f; // Movement speed
    private const float POSITION_THRESHOLD = 0.01f; // Small buffer to prevent float precision issues

    void Update()
    {
        if (targetSpot != null && !HasReachedTarget())
        {
            // Move toward the target position smoothly
            transform.position = Vector3.MoveTowards(transform.position, targetSpot.position, moveSpeed * Time.deltaTime);
        }
    }

    // Public method to set the target spot and movement speed dynamically
    public void SetTarget(Transform newTarget, float speed)
    {
        targetSpot = newTarget;
        moveSpeed = speed;
    }

    // Check if the object has reached the target position
    private bool HasReachedTarget()
    {
        return Vector3.SqrMagnitude(transform.position - targetSpot.position) < POSITION_THRESHOLD * POSITION_THRESHOLD;
    }
}

