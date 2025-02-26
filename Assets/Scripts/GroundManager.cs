using UnityEngine;

[ExecuteAlways] // Runs in Editor and Play Mode
public class GroundManager : MonoBehaviour
{
    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 20;
    private Transform groundTransform;

    void Awake()
    {
        groundTransform = transform;
        AdjustGroundSize(); // Ensures the correct size in game
    }

    // This will run automatically whenever the script values are changed in the Inspector
    void OnValidate()
    {
        if (!Application.isPlaying) // Only apply in Editor mode
        {
            AdjustGroundSize();
        }
    }

    private void AdjustGroundSize()
    {
        transform.localScale = new Vector3(gridWidth, gridHeight, 1);
        transform.position = Vector3.zero; // Ensure position is fixed
    }
}
