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

    [Header("UI Manager")]
    public UIManager uiManager;

    private float currentSpeed = 0f;
    private float steerValue;
    private float throttleValue;
    private float brakeValue;

    public InputAction steerAction;
    public InputAction throttleAction;
    public InputAction brakeAction;

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
        // INPUT
        // ---------------------------
        steerValue = steerAction.ReadValue<float>();
        float rawThrottle = throttleAction.ReadValue<float>();
        float rawBrake = brakeAction.ReadValue<float>();

        // --- Keyboard fallback for testing ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) rawThrottle = -1f; // W → full forward
            if (Keyboard.current.sKey.isPressed) rawBrake = -1f;    // S → full brake
            if (Keyboard.current.aKey.isPressed) steerValue = -1f;  // A → left
            if (Keyboard.current.dKey.isPressed) steerValue = 1f;   // D → right
        }

        throttleValue = Mathf.Clamp01((1f - rawThrottle) / 2f);
        brakeValue = Mathf.Clamp01((1f - rawBrake) / 2f);

        // ---------------------------
        // SPEED LOGIC
        // ---------------------------
        if (throttleValue > 0f)
            currentSpeed += throttleValue * acceleration * Time.deltaTime;
        else
            currentSpeed -= deceleration * Time.deltaTime;

        if (brakeValue > 0f)
            currentSpeed -= brakeValue * brakeForce * Time.deltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // ---------------------------
        // MOVEMENT
        // ---------------------------
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
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
        // Vruchtbaar: also 100 → 0 (NO reset)
        // ---------------------------
        if (vruchtbaar > 0f)
        {
            vruchtbaar -= progressSpeed * Time.deltaTime;
            if (vruchtbaar < 0f) vruchtbaar = 0f;
        }

        Debug.Log(
            $"Speed: {currentSpeed:F2}, Vruchtbaar: {vruchtbaar:F1}, ArcVal: {fakeValue:F1}, Fill: {mappedFill:F2}"
        );
    }
}
