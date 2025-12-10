using UnityEngine;

public class PowerUpSpin : MonoBehaviour
{
    [Header("Spin Settings")]
    public float spinSpeed = 180f;           // Degrees per second
    public Vector3 spinAxis = Vector3.up;    // Spin around Y-axis (vertical)
    public bool bobUpDown = true;            // Optional floating bob
    public float bobHeight = 0.3f;           // How high/low to bob
    public float bobSpeed = 2f;              // Bob speed

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Spin in place
        transform.Rotate(spinAxis * spinSpeed * Time.deltaTime, Space.World);

        // Optional: Bob up and down (floating effect)
        if (bobUpDown)
        {
            float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = startPos + Vector3.up * bobOffset;
        }
    }
}