using UnityEngine;

public class FollowCar : MonoBehaviour
{
    public Transform seatAnchor;

    void LateUpdate()
    {
        transform.position = seatAnchor.position;
        transform.rotation = seatAnchor.rotation;
    }
}
