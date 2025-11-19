using UnityEngine;
using UnityEngine.InputSystem;

public class G29Controller : MonoBehaviour
{
    [Header("Car Settings")]
    public float baseSpeed = 5f;          // Always moves forward
    public float boostSpeed = 15f;        // Gas adds this amount
    public float boostAcceleration = 10f; // Smooth boost
    public float steeringAngle = 45f;

    private float currentBoost = 0f;
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
        steerValue = steerAction.ReadValue<float>();
        throttleValue = Mathf.Clamp01(throttleAction.ReadValue<float>()); // 0–1
        brakeValue = Mathf.Clamp01(brakeAction.ReadValue<float>());       // 0–1

        // Desired boost: throttle increases, brake reduces
        float targetBoost = (throttleValue - brakeValue) * boostSpeed;

        // Prevent boost from going negative
        if (targetBoost < 0)
            targetBoost = 0;

        currentBoost = Mathf.MoveTowards(
            currentBoost,
            targetBoost,
            boostAcceleration * Time.deltaTime
        );

        // Final speed: ALWAYS at least baseSpeed
        float finalSpeed = baseSpeed + currentBoost;

        transform.Translate(Vector3.forward * finalSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, steerValue * steeringAngle * Time.deltaTime);
    }
}
