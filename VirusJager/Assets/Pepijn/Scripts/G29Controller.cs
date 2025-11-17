using UnityEngine;
using UnityEngine.InputSystem;

public class G29Controller : MonoBehaviour
{
    [Header("Car Settings")]
    public float baseSpeed = 5f;           // auto beweegt altijd vooruit
    public float boostSpeed = 12f;         // max snelheid tijdens gas geven
    public float acceleration = 5f;        // hoe snel snelheid verandert
    public float steeringAngle = 45f;      // maximale stuurhoek

    private float currentSpeed = 0f;
    private float steerValue;
    private float throttleValue;
    private float brakeValue;

    // Input Actions
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
        throttleValue = throttleAction.ReadValue<float>();
        brakeValue = brakeAction.ReadValue<float>();

        // ---- SPEED MODEL ----
        float targetSpeed = baseSpeed;

        if (throttleValue > 0.05f) // gas ingedrukt
            targetSpeed = boostSpeed;

        if (brakeValue > 0.05f) // rem ingedrukt
            targetSpeed = 0f;

        // vloeiend naar target speed bewegen
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // ---- MOVEMENT ----
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // ---- REALISTISCH STUREN ----
        // Alleen kunnen sturen als de auto beweegt
        float speedFactor = Mathf.InverseLerp(0f, boostSpeed, currentSpeed);

        float rotation = steerValue * steeringAngle * speedFactor;
        transform.Rotate(Vector3.up, rotation * Time.deltaTime);
    }
}
