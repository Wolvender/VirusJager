using UnityEngine;
using UnityEngine.InputSystem;

public class G29Controller : MonoBehaviour
{
    [Header("Car Settings")]
    public float baseSpeed = 5f;          // Always moves forward
    public float boostSpeed = 15f;        // Extra speed when gas is pressed
    public float boostAcceleration = 10f; // How fast boost changes
    public float steeringAngle = 45f;

    private float currentBoost = 0f;
    private float steerValue;
    private float throttleValue;

    public InputAction steerAction;
    public InputAction throttleAction;

    void OnEnable()
    {
        steerAction.Enable();
        throttleAction.Enable();
    }

    void OnDisable()
    {
        steerAction.Disable();
        throttleAction.Disable();
    }

    void Update()
    {
        // Lees stuur
        steerValue = steerAction.ReadValue<float>();

        // Lees throttle en normaliseer van G29 (los = 1, ingedrukt = -1) naar 0..1
        float rawThrottle = throttleAction.ReadValue<float>();   // 1..-1
        throttleValue = Mathf.Clamp01((1f - rawThrottle) / 2f); // 0..1

        // Bereken gewenste boost op basis van throttle
        float targetBoost = throttleValue * boostSpeed;

        // Smoothly bewegen naar target boost
        currentBoost = Mathf.MoveTowards(
            currentBoost,
            targetBoost,
            boostAcceleration * Time.deltaTime
        );

        // Final speed: altijd minimaal baseSpeed, plus boost
        float finalSpeed = baseSpeed + currentBoost;

        // Beweeg en draai de auto
        transform.Translate(Vector3.forward * finalSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, steerValue * steeringAngle * Time.deltaTime);

        // Debug: laat throttle, boost en snelheid zien
        Debug.Log($"Throttle: {throttleValue:F2}, Boost: {currentBoost:F2}, Speed: {finalSpeed:F2}");
    }
}
