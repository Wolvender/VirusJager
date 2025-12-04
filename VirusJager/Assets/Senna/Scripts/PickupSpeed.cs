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
    private bool alreadyUsed = false;          // ← prevents double-trigger

    private void Awake()
    {
        // Setup audio
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // Force trigger
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Safety checks
        if (alreadyUsed) return;
        if (!other.CompareTag("Player")) return;

        carController = other.GetComponent<G29Controller>();
        if (carController == null) return;

        alreadyUsed = true;                     // ← stops double pickup

        // 2. Play sound
        if (boostSound) audioSource.PlayOneShot(boostSound);

        // 3. Apply boost + guarantee restore + destroy
        StartCoroutine(ApplyAndRestoreBoost());

        // 4. Visual disappear immediately
        var mr = GetComponent<MeshRenderer>();
        if (mr) mr.enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    private IEnumerator ApplyAndRestoreBoost()
    {
        // Grab REAL base values right now
        float originalMaxSpeed = carController.maxSpeed;
        float originalAccel = carController.acceleration;

        // Apply boost
        carController.maxSpeed *= speedMultiplier;
        carController.acceleration *= accelerationMultiplier;

        // Wait exact time
        yield return new WaitForSeconds(boostDuration);

        // FORCE restore – this wins over everything
        carController.maxSpeed = originalMaxSpeed;
        carController.acceleration = originalAccel;

        // Destroy the pickup object (100% guaranteed)
        Destroy(gameObject);
    }
}