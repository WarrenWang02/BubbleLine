using UnityEngine;

public class FaceCameraAll : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        GameObject[] ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        
        foreach (GameObject ingredient in ingredients)
        {
            ingredient.transform.LookAt(mainCamera.transform);
            ingredient.transform.Rotate(0, 180f, 0);  // Adjust if needed
        }
    }
}
