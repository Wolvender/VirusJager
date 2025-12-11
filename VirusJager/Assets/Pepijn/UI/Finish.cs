using UnityEngine;

public class Finish : MonoBehaviour
{
    public UIManage uiManager;
    private bool raceEnded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (raceEnded) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("entered");
            raceEnded = true;
            uiManager.ShowWinScreen();

            PlayerCelebration celebration = other.GetComponent<PlayerCelebration>();
            if (celebration != null)
            {
                Debug.Log("start");
                celebration.StartCelebration();
            }
        }
        else if (other.CompareTag("Sperm"))
        {
            raceEnded = true;
            uiManager.ShowGameOver();
        }
    }
}
