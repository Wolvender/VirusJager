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
    public float minFill = 0.15f;
    public float maxFill = 0.85f;

    [Header("UI Manager")]
    public UIManager uiManager;

    private float currentFuel;
    private float currentBoost = 0f;
    private float currentBaseSpeed; // <-- IMPORTANT
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
        currentBaseSpeed = baseSpeed;

        if (fuelSlider != null)
            fuelSlider.maxValue = maxFuel;
    }

    void Update()
    {
        // ---------------------------
        // INPUT
        // ---------------------------
        steerValue = steerAction.ReadValue<float>();
        float rawThrottle = throttleAction.ReadValue<float>();
        throttleValue = Mathf.Clamp01((1f - rawThrottle) / 2f);

        // ---------------------------
        // BOOST
        // ---------------------------
        float targetBoost = (currentFuel > 0f) ? throttleValue * boostSpeed : 0f;

        currentBoost = Mathf.MoveTowards(
            currentBoost,
            targetBoost,
            boostAcceleration * Time.deltaTime
        );

        // ---------------------------
        // BASE SPEED REDUCTION WHEN EMPTY
        // ---------------------------
        if (currentFuel > 0f)
        {
            currentBaseSpeed = baseSpeed;
        }
        else
        {
            currentBaseSpeed = Mathf.MoveTowards(currentBaseSpeed, 0, 2f * Time.deltaTime);
        }

        float finalSpeed = currentBaseSpeed + currentBoost;

        // ---------------------------
        // MOVEMENT
        // ---------------------------
        transform.Translate(Vector3.forward * finalSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, steerValue * steeringAngle * Time.deltaTime);

        // ---------------------------
        // FUEL DRAIN
        // ---------------------------
        if (finalSpeed > 0.1f && currentFuel > 0f)
        {
            float drain = fuelDrainNormal;

            float boostPercent = currentBoost / boostSpeed;
            drain += boostPercent * fuelDrainBoost;

            currentFuel -= drain * Time.deltaTime;
            currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        }

        // ---------------------------
        // UI UPDATE
        // ---------------------------
        if (fuelSlider != null)
            fuelSlider.value = currentFuel;

        if (fuelFillImage != null)
        {
            float fuelPercent = currentFuel / maxFuel;
            float mappedFill = Mathf.Lerp(minFill, maxFill, fuelPercent);
            fuelFillImage.fillAmount = mappedFill;
        }

        // ---------------------------
        // GAME OVER CHECK
        // ---------------------------
        if (finalSpeed <= 0.01f && currentFuel <= 0f)
        {
            if (uiManager != null)
                uiManager.ShowGameOver();
        }

        Debug.Log($"Fuel: {currentFuel:F1}, Final Speed: {finalSpeed:F2}, Base Speed: {currentBaseSpeed:F2}");
    }
}
