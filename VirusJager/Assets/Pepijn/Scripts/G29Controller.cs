using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class G29Controller : MonoBehaviour
{
    [Header("Car Settings")]
    public float acceleration = 12f;
    public float deceleration = 8f;
    public float brakeForce = 45f;
    public float maxSpeed = 20f;
    public float steeringAngle = 45f;
    public float CurrentSpeed => currentSpeed;

    [Header("UI Bar Settings")]
    public Image fuelFillImage;
    public float minFill = 0.15f;
    public float maxFill = 0.85f;
    public float progressSpeed = 5f;
    private float fakeValue = 100f;

    [Header("Tracked Variable")]
    public float vruchtbaar = 100f;

    [Header("Collision Check")]
    public float frontCheckDistance = 1f;
    public LayerMask wallLayer;

    private float currentSpeed = 0f;
    private float steerValue;
    private float throttleValue;
    private float brakeValue;

    public InputAction steerAction;
    public InputAction throttleAction;
    public InputAction brakeAction;

    [Header("Ground Check")]
    public float groundRayLength = 1.0f;
    public LayerMask groundLayer;
    private bool isGrounded;

    void OnEnable()
    {
        steerAction.Enable();
        throttleAction.Enable();
        brakeAction.Enable();
    }

    void OnDisable()
    {
        steerAction.Disable();
        throttleAction.Disable();
        brakeAction.Disable();
    }

    void Update()
    {
        // ---------------------------
        // GROUND CHECK
        // ---------------------------
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, groundRayLength, groundLayer);

        // ---------------------------
        // INPUT
        // ---------------------------
        steerValue = steerAction.ReadValue<float>();
        float rawThrottle = throttleAction.ReadValue<float>();
        float rawBrake = brakeAction.ReadValue<float>();

        // --- Keyboard fallback for testing ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) rawThrottle = -1f; // W → forward
            if (Keyboard.current.sKey.isPressed) rawBrake = -1f;    // S → brake
            if (Keyboard.current.aKey.isPressed) steerValue = -1f;  // A → left
            if (Keyboard.current.dKey.isPressed) steerValue = 1f;   // D → right
        }

        throttleValue = Mathf.Clamp01((1f - rawThrottle) / 2f);
        brakeValue = Mathf.Clamp01((1f - rawBrake) / 2f);

        // ---------------------------
        // SPEED LOGIC – Only accelerate on ground!
        // ---------------------------
        if (isGrounded)
        {
            if (throttleValue > 0f)
                currentSpeed += throttleValue * acceleration * Time.deltaTime;
            else
                currentSpeed -= deceleration * Time.deltaTime;
        }
        else
        {
            // In air: slow down slightly (optional)
            currentSpeed -= deceleration * 0.2f * Time.deltaTime;
        }

        if (brakeValue > 0f)
            currentSpeed -= brakeValue * brakeForce * Time.deltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // ---------------------------
        // COLLISION CHECK
        // ---------------------------
        bool hitWall = Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, frontCheckDistance, wallLayer);

        // ---------------------------
        // MOVEMENT
        // ---------------------------
        if (!hitWall)
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = 0f; // optional: stop the car if it hits a wall
        }

        transform.Rotate(Vector3.up, steerValue * steeringAngle * Time.deltaTime);

        // ---------------------------
        // UI ARC: slow 100 → 0
        // ---------------------------
        if (fakeValue > 0f)
        {
            fakeValue -= progressSpeed * Time.deltaTime;
            if (fakeValue < 0f) fakeValue = 0f;
        }

        float percent = fakeValue / 100f;
        float mappedFill = Mathf.Lerp(minFill, maxFill, percent);

        if (fuelFillImage != null)
            fuelFillImage.fillAmount = mappedFill;

        // ---------------------------
        // Vruchtbaar: also 100 → 0
        // ---------------------------
        if (vruchtbaar > 0f)
        {
            vruchtbaar -= progressSpeed * Time.deltaTime;
            if (vruchtbaar < 0f) vruchtbaar = 0f;
        }



    }

    // ---------------------------
    // GROUND DEBUG DRAW (optional)
    // ---------------------------
    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.2f,
                        transform.position + Vector3.up * 0.2f + Vector3.down * groundRayLength);

        // Draw front collision ray
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f,
                        transform.position + Vector3.up * 0.5f + transform.forward * frontCheckDistance);
    }
}
