using UnityEngine;

public class IngredientMover : MonoBehaviour
{
    [SerializeField] private Transform targetSpot;
    [SerializeField] private float moveSpeed = 2f;
    private const float POSITION_THRESHOLD = 0.01f;
    
    private Vector3 lastPosition;  // Track last position for stuck detection
    private float stuckTimer = 0f; // Timer for stuck detection
    private const float STUCK_TIME_LIMIT = 2.0f; // Reset if stuck for 2 seconds

    void Update()
    {
        if (targetSpot != null && !HasReachedTarget())
        {
            transform.position = Vector3.MoveTowards(transform.position, targetSpot.position, moveSpeed * Time.deltaTime);

            // Detect if stuck
            if (Vector3.Distance(transform.position, lastPosition) < 0.001f) 
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > STUCK_TIME_LIMIT)
                {
                    Debug.LogWarning($"{gameObject.name} might be stuck!");
                    stuckTimer = 0;
                    //transform.position += Vector3.up * 0.1f;  // Nudge upwards to try unsticking
                }
            }
            else
            {
                stuckTimer = 0;  // Reset timer if moving
            }

            lastPosition = transform.position;
        }
    }

    public void SetTarget(Transform newTarget, float speed)
    {
        // Prevent redundant setting if already set
        if (targetSpot == newTarget) return;  

        targetSpot = newTarget;
        moveSpeed = speed;
        Debug.Log($"{gameObject.name} is now moving to {newTarget.name} at speed {speed}");
    }

    private bool HasReachedTarget()
    {
        return Vector3.SqrMagnitude(transform.position - targetSpot.position) < POSITION_THRESHOLD * POSITION_THRESHOLD;
    }
}
