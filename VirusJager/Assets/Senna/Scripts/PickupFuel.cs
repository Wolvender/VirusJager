using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float fuel = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            G29Controller controller = other.GetComponent<G29Controller>();

            if (controller != null)
            {
                controller.AddFuel(fuel);
                Debug.Log("Fuel added: " + fuel);
            }

            Destroy(gameObject);
        }
    }
}
