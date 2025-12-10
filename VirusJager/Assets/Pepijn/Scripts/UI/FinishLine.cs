using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public GameObject raceCamera;
    public GameObject finishCamera;
    public Animator finishAnimator;
    public float delayToChangeScene = 3f;

    private bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag("Player")) return;

        finished = true;

        // Turn off race cam and enable finish camera
        raceCamera.SetActive(false);
        finishCamera.SetActive(true);

        // Disable driving script
        other.GetComponent<G29Controller>().enabled = false;

        // Start animation
        finishAnimator.SetTrigger("Finish");

        // Load next scene / UI popup / or anything else
        Invoke(nameof(NextScene), delayToChangeScene);
    }

    private void NextScene()
    {
        SceneManager.LoadScene("NextScene"); // of results scherm
    }
}
