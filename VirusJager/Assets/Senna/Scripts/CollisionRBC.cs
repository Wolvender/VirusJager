using UnityEngine;

public class CollisionRBC : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player hit RBC!");
            ScoreManager.Instance.MinusScore(10);
            Object.Destroy(gameObject, 3f); // Delay to finish animation
            //add oxygen 
        }
    }
}
