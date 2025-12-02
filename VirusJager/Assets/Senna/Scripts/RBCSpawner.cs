using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    public Transform enemyFolder; // Parent that contains all enemies

    void Start()
    {
        ActivateEnemies();
    }

    void ActivateEnemies()
    {
        foreach (Transform child in enemyFolder)
        {
            child.gameObject.SetActive(true);
        }
    }
}
