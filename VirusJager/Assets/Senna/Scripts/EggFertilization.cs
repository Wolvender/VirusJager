using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.AI;

public class EggFertilization : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject losePanel;
    public GameObject winPanel;

    [Header("Win Panel Stuff")]
    public TextMeshProUGUI resultText;
    public Animator eggAnimator;
    public string fertilizationTrigger = "Fertilize";

    [Header("What to destroy on game end")]
    public string spermTag = "Sperm";
    public List<GameObject> extraObjectsToDestroy = new List<GameObject>();

    private bool gameHasEnded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gameHasEnded) return;

        if (other.CompareTag("Sperm"))
        {
            GameOverLose();
        }
        else if (other.CompareTag("Player"))
        {
            GameOverWin();
        }

        // Stop the agent
        if (other.TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.isStopped = true;
        }
    }

    void GameOverLose()
    {
        if (gameHasEnded) return;
        gameHasEnded = true;

        Debug.Log("You lost!");

        // SAFE version – won't throw exceptions if you didn't assign the panels yet
        if (losePanel != null) losePanel.SetActive(true);
        else Debug.Log("[EggFertilization] losePanel not assigned – skipping show lose screen");

        Time.timeScale = 0f;                       // ← this will now run
        DestroyAllSpermAndCleanup();               // ← this will now run
    }

    void GameOverWin()
    {
        if (gameHasEnded) return;
        gameHasEnded = true;

        Debug.Log("You won!");

        if (winPanel != null) winPanel.SetActive(true);
        else Debug.Log("[EggFertilization] winPanel not assigned – skipping show win screen");

        Time.timeScale = 0f;
        DestroyAllSpermAndCleanup();

        if (eggAnimator != null)
            eggAnimator.SetTrigger(fertilizationTrigger);

    }

    void DestroyAllSpermAndCleanup()
    {
        Debug.Log("🔥 NUCLEAR SPERM CLEANUP STARTED 🔥");

        // Method 1: Tag-based (your original)
        GameObject[] sperms = GameObject.FindGameObjectsWithTag("Sperm");
        int count1 = 0;
        foreach (GameObject sperm in sperms)
        {
            if (sperm != null)
            {
                DestroyImmediate(sperm);
                count1++;
            }
        }
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count2 = 0;
        foreach (GameObject obj in allObjects)
        {
            if (obj != null &&
                (obj.name.Contains("Sperm") || obj.name.Contains("Sperma") || obj.name.Contains("dak")))
            {
                DestroyImmediate(obj);
                count2++;
            }
        }

        // Method 3: Disable ALL NavMeshAgents instantly (stops movement NOW)
        NavMeshAgent[] allAgents = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agent in allAgents)
        {
            if (agent != null && agent.gameObject != this.gameObject)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }
        }

        // Extra objects
        foreach (GameObject obj in extraObjectsToDestroy)
        {
            if (obj != null) DestroyImmediate(obj);
        }

        Debug.Log($"💀 DESTROYED {count1} tagged + {count2} name-based = TOTAL {count1 + count2} sperm. ALL AGENTS DISABLED.");
    }

    // Button method: Try Again
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}