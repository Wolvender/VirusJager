using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FuelPickup : MonoBehaviour
{
    [Header("Fuel Settings")]
    public float fuelToAdd = 100f;        // How much this pickup gives
    public float maxFuel = 100f;          // Maximum fuel the car can have

    [Header("Audio & Visual")]
    public AudioClip pickupSound;

    private AudioSource audioSource;
    private bool alreadyUsed = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyUsed) return;
        if (!other.CompareTag("Player")) return;

        G29Controller car = other.GetComponent<G29Controller>();
        if (car == null) return;

        alreadyUsed = true;

        if (pickupSound) audioSource.PlayOneShot(pickupSound);

        // Add fuel and cap at 100 (hard-coded max, same as everywhere else)
        car.vruchtbaar += fuelToAdd;
        car.vruchtbaar = Mathf.Clamp(car.vruchtbaar, 0f, 100f);

        // Disappear
        var mr = GetComponent<MeshRenderer>();
        if (mr) mr.enabled = false;
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, pickupSound ? pickupSound.length : 0.3f);
    }
}