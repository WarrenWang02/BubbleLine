using UnityEngine;

public class DeadZoneUnderGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name + " entered DeadZone and was destroyed.");
        Destroy(other.gameObject);
    }
}
