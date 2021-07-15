using UnityEngine;

public class PositionCamera : MonoBehaviour
{
    [SerializeField]
    Transform cameraPosition;

    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
