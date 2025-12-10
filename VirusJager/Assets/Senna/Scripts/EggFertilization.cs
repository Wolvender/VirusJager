using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EggFertilization : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject losePanel;
    public GameObject winPanel;

    [Header("Text (Optional - auto-finds if empty)")]
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;

    [Header("Win Animation")]
    public Animator eggAnimator;
    public string fertilizationTrigger = "Fertilize";

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
    }

    void GameOverWin()
    {
        if (gameHasEnded) return;
        gameHasEnded = true;

        PauseGame();

        if (winPanel != null) winPanel.SetActive(true);
        if (winText == null) winText = winPanel?.GetComponentInChildren<TextMeshProUGUI>();
        if (winText != null) winText.text = "SUCCESS!\nYou fertilized the egg!";

        if (eggAnimator != null)
            eggAnimator.SetTrigger(fertilizationTrigger);
    }

    void GameOverLose()
    {
        if (gameHasEnded) return;
        gameHasEnded = true;

        PauseGame();

        if (losePanel != null) losePanel.SetActive(true);
        if (loseText == null) loseText = losePanel?.GetComponentInChildren<TextMeshProUGUI>();
        if (loseText != null) loseText.text = "GAME OVER!\nA sperm got there first!";
    }

    // THIS IS THE ONLY THING WE NEED TO PAUSE THE GAME
    private void PauseGame()
    {
        Time.timeScale = 0f;  // Everything stops (including your RBCNavRacer script if it uses deltaTime)
    }

    // BUTTONS – assign these in the Inspector on your Try Again buttons
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}