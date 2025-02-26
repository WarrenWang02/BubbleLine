using UnityEngine;

[ExecuteAlways] // Runs in both Editor and Play Mode
public class GroundManager : MonoBehaviour
{
    [SerializeField, Min(2)] private int gridWidth = 20;  // Min 2, even only
    [SerializeField, Min(2)] private int gridHeight = 20; // Min 2, even only
    private Transform groundTransform;

    void Awake()
    {
        groundTransform = transform;
        AdjustGroundSize();
    }

    // This ensures values stay even and updates ground size in Editor
    void OnValidate()
    {
        if (!Application.isPlaying) // Prevents runtime interference
        {
            gridWidth = Mathf.Max(2, gridWidth / 2 * 2);   // Forces even, min 2
            gridHeight = Mathf.Max(2, gridHeight / 2 * 2); // Forces even, min 2
            AdjustGroundSize();
        }
    }

    public int GetGridWidth() => gridWidth;
    public int GetGridHeight() => gridHeight;

    private void AdjustGroundSize()
    {
        transform.localScale = new Vector3(gridWidth, gridHeight, 1);
        transform.position = Vector3.zero; // Keep fixed
    }
}
