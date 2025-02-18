using UnityEngine;
using System.Collections.Generic;

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
        GameObject[] inGameUIs = GameObject.FindGameObjectsWithTag("InGameUI");

        // Combine arrays into one list
        List<GameObject> allObjects = new List<GameObject>();
        allObjects.AddRange(ingredients);
        allObjects.AddRange(inGameUIs);

        // Apply the transformation to all detected objects
        foreach (GameObject obj in allObjects)
        {
            obj.transform.LookAt(mainCamera.transform);
            obj.transform.Rotate(0, 180f, 0);  // Adjust if needed
        }
    }
}
