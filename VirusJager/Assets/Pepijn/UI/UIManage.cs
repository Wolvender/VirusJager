using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Only needed if you use the auto-button assignment below

public class UIManage : MonoBehaviour
{
    [Header("Win Panels (Randomly picks one)")]
    public GameObject winPanel1;
    public GameObject winPanel2;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;

    [Header("Optional: Auto-assign Restart Buttons (recommended)")]
    public Button restartButtonOnGameOver;
    public Button[] restartButtonsOnWinPanels; // Drag all restart buttons from both win panels here

    private void Awake()
    {
        // Hide everything at the start
        if (winPanel1) winPanel1.SetActive(false);
        if (winPanel2) winPanel2.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Automatically connect restart buttons (super convenient!)
        if (restartButtonOnGameOver != null)
            restartButtonOnGameOver.onClick.AddListener(RestartLevel);

        if (restartButtonsOnWinPanels != null)
        {
            foreach (var btn in restartButtonsOnWinPanels)
            {
                if (btn != null)
                    btn.onClick.AddListener(RestartLevel);
            }
        }
    }

    public void ShowWinScreen()
    {
        int random = Random.Range(0, 2); // 0 or 1

        if (random == 0 && winPanel1 != null)
        {
            winPanel1.SetActive(true);
        }
        else if (winPanel2 != null)
        {
            winPanel2.SetActive(true);
        }

        Time.timeScale = 0f; // Pause the game when you win
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f; // Pause the game on loss
    }

    // This gets called by any Restart / Play Again button
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Always unpause first!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}