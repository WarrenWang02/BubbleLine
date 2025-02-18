using UnityEngine;
using System.Collections.Generic;

public class FaceCameraAll : MonoBehaviour
{
    private Camera mainCamera;
    private List<GameObject> allObjects = new List<GameObject>();

    private void Start()
    {
        mainCamera = Camera.main;
        GameObject[] ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        GameObject[] inGameUIs = GameObject.FindGameObjectsWithTag("InGameUI");
        
        allObjects.AddRange(ingredients);
        allObjects.AddRange(inGameUIs);
    }

    private void Update()
    {
        // Apply the transformation to all detected objects
        foreach (GameObject obj in allObjects)
        {
            if (obj != null) // ✅ Avoid errors if objects are destroyed
            {
                obj.transform.LookAt(mainCamera.transform);
                obj.transform.Rotate(0, 180f, 0);
            }
        }
    }
}
