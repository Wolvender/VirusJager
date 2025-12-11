using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FuelPickup : MonoBehaviour
{
    [Header("Fuel Settings")]
    public float fuelToAdd = 100f;
    public float maxFuel = 100f;

    [Header("Audio & Visual")]
    public AudioClip pickupSound;

    private AudioSource audioSource;
    private bool alreadyUsed = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        // 🔥 FIX #1: Add kinematic Rigidbody (CRITICAL!)
        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyUsed) return;

        if (!other.CompareTag("Player")) return;

        // 🔥 FIX #2: Better controller detection (like SpeedBoost)
        G29Controller car = other.GetComponentInParent<G29Controller>() ??
                           other.GetComponent<G29Controller>() ??
                           other.GetComponentInChildren<G29Controller>();

        if (car == null)
        {
            Debug.LogWarning("[FuelPickup] Player touched fuel but no G29Controller found!");
            return;
        }

        alreadyUsed = true;

        // 🔥 FIX #3: Debug confirmation
        Debug.Log("[FuelPickup] FUEL PICKED UP!");

        if (pickupSound) audioSource.PlayOneShot(pickupSound);

        // Add fuel
        car.vruchtbaar += fuelToAdd;
        car.vruchtbaar = Mathf.Clamp(car.vruchtbaar, 0f, maxFuel);

        // Disappear
        var mr = GetComponent<MeshRenderer>();
        if (mr) mr.enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, pickupSound ? pickupSound.length : 0.3f);
    }
}