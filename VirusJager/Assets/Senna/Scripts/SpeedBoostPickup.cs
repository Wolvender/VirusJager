using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class SpeedBoostPickup : MonoBehaviour
{
    [Header("Nitro Boost")]
    public float speedMultiplier = 1.7f;
    public float accelerationMultiplier = 6.5f;
    public float boostDuration = 0.45f;

    [Header("Audio")]
    public AudioClip boostSound;

    private AudioSource audioSource;
    private G29Controller carController;
    private bool alreadyUsed = false;

    private void Awake()
    {
        // Setup audio
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // Force trigger + add kinematic Rigidbody (required for trigger to work!)
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Prevent double pickup
        if (alreadyUsed) return;

        // Only react to Player tag
        if (!other.CompareTag("Player")) return;

        // Find G29Controller anywhere in the player hierarchy
        carController = other.GetComponentInParent<G29Controller>() ?? other.GetComponentInChildren<G29Controller>();

        // CRITICAL: Only proceed if we actually found the controller!
        if (carController == null)
        {
            Debug.LogWarning("[SpeedBoost] Player touched boost but has no G29Controller in hierarchy!");
            return;
        }

        alreadyUsed = true;
        Debug.Log("[SpeedBoost] NITRO PICKED UP! BOOST ACTIVATED!");

        // Play sound
        if (boostSound && audioSource)
            audioSource.PlayOneShot(boostSound);

        // Apply boost
        StartCoroutine(ApplyAndRestoreBoost());

        // Visual disappear
        var mr = GetComponent<MeshRenderer>();
        if (mr) mr.enabled = false;

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }

    private IEnumerator ApplyAndRestoreBoost()
    {
        // Store original values
        float originalMaxSpeed = carController.maxSpeed;
        float originalAccel = carController.acceleration;

        // Apply boost
        carController.maxSpeed *= speedMultiplier;
        carController.acceleration *= accelerationMultiplier;

        // Wait
        yield return new WaitForSeconds(boostDuration);

        // Restore original values
        carController.maxSpeed = originalMaxSpeed;
        carController.acceleration = originalAccel;

        // Self-destruct
        Destroy(gameObject);
    }
}   