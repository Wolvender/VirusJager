using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringWheelAnimator : MonoBehaviour
{
    [Header("Wheel Settings")]
    public Transform wheelModel;
    public InputAction steerAction;

    public float maxWheelRotation = 450f;

    // Kies de juiste rotatie-as
    public enum RotationAxis { X, Y, Z }
    public RotationAxis axis = RotationAxis.Z;

    private Quaternion initialRotation;

    void OnEnable()
    {
        steerAction.Enable();
    }

    void OnDisable()
    {
        steerAction.Disable();
    }

    void Start()
    {
        // Bewaar de originele rotatie van het stuur
        initialRotation = wheelModel.localRotation;
    }

    void Update()
    {
        float steerValue = steerAction.ReadValue<float>(); // -1 tot 1
        float rotationAmount = steerValue * maxWheelRotation;

        // Bouw the nieuwe rotatie op basis van gekozen as
        Quaternion rot = Quaternion.identity;

        switch (axis)
        {
            case RotationAxis.X:
                rot = Quaternion.Euler(rotationAmount, 0, 0);
                break;
            case RotationAxis.Y:
                rot = Quaternion.Euler(0, rotationAmount, 0);
                break;
            case RotationAxis.Z:
                rot = Quaternion.Euler(0, 0, rotationAmount);
                break;
        }

        // Combineer met originele rotatie
        wheelModel.localRotation = initialRotation * rot;
    }
}
