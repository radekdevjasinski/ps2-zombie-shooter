using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
