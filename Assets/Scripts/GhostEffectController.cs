using UnityEngine;

public class GhostEffectController : MonoBehaviour
{
    private Material originalMaterial;
    private Renderer objectRenderer;
    public Material ghostMaterial; // Assign the ghost shader material in Inspector

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material; // Store original material
        }
    }

    public void EnableGhostEffect()
    {
        if (objectRenderer != null && ghostMaterial != null)
        {
            objectRenderer.material = ghostMaterial; // Switch to ghost shader
        }
    }

    public void DisableGhostEffect()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material = originalMaterial; // Restore original look
        }
    }
}
