using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class G29Controller : MonoBehaviour
{
    [Header("Car Settings")]
    public float baseSpeed = 5f;
    public float boostSpeed = 15f;
    public float boostAcceleration = 10f;
    public float steeringAngle = 45f;

    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float fuelDrainNormal = 2f;
    public float fuelDrainBoost = 8f;
    public Slider fuelSlider;
    public Image fuelFillImage;

    [Header("Fuel Gauge Arc Settings")]
    public float minFill = 0.15f; // fill amount when empty
    public float maxFill = 0.85f; // fill amount when full

    private float currentFuel;
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

    void Start()
    {
        currentFuel = maxFuel;

        if (fuelSlider != null)
            fuelSlider.maxValue = maxFuel;
    }

    void Update()
    {
        // -------------------------------------
        // INPUT
        // -------------------------------------
        steerValue = steerAction.ReadValue<float>();
        float rawThrottle = throttleAction.ReadValue<float>();
        throttleValue = Mathf.Clamp01((1f - rawThrottle) / 2f);

        // -------------------------------------
        // BOOST CALCULATION
        // -------------------------------------
        float targetBoost = throttleValue * boostSpeed;

        if (currentFuel <= 0f)
            targetBoost = 0f; // no boost when out of fuel

        currentBoost = Mathf.MoveTowards(
            currentBoost,
            targetBoost,
            boostAcceleration * Time.deltaTime
        );

        // -------------------------------------
        // SPEED CONTROL
        // -------------------------------------
        float effectiveBaseSpeed = baseSpeed;

        if (currentFuel <= 0f)
        {
            // smoothly reduce base speed to 0 when out of fuel
            effectiveBaseSpeed = Mathf.MoveTowards(effectiveBaseSpeed, 0, 2f * Time.deltaTime);
        }

        float finalSpeed = effectiveBaseSpeed + currentBoost;

        // -------------------------------------
        // MOVEMENT
        // -------------------------------------
        transform.Translate(Vector3.forward * finalSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, steerValue * steeringAngle * Time.deltaTime);

        // -------------------------------------
        // FUEL DRAIN
        // -------------------------------------
        if (finalSpeed > 0.1f && currentFuel > 0f)
        {
            float drain = fuelDrainNormal;

            float boostPercent = currentBoost / boostSpeed;
            drain += boostPercent * fuelDrainBoost;

            currentFuel -= drain * Time.deltaTime;
            currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        }

        // -------------------------------------
        // UPDATE UI
        // -------------------------------------
        if (fuelSlider != null)
            fuelSlider.value = currentFuel;

        if (fuelFillImage != null)
        {
            float fuelPercent = currentFuel / maxFuel;

            // map 0–1 fuel to minFill–maxFill arc
            float mappedFill = Mathf.Lerp(minFill, maxFill, fuelPercent);

            fuelFillImage.fillAmount = mappedFill;
        }

        Debug.Log($"Fuel: {currentFuel:F1}, Speed: {finalSpeed:F2}");
    }
}
