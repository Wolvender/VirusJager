using UnityEngine;

public class UIManage : MonoBehaviour
{
    public GameObject winPanel1;
    public GameObject winPanel2;
    public GameObject gameOverPanel;

    public void ShowWinScreen()
    {
        int random = Random.Range(0, 2); // 0 or 1

        if (random == 0)
        {
            winPanel1.SetActive(true);
        }
        else
        {
            winPanel2.SetActive(true);
        }
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
