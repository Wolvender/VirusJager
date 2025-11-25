using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float boostAmount = 10f;
    public float boostDuration = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("boost: " + boostAmount);
            G29Controller controller = other.GetComponent<G29Controller>();

            if (controller != null)
            {
                controller.ApplyTemporaryBoost(boostAmount, boostDuration);
                
            }

            Destroy(gameObject); // remove pickup
        }
    }
}
