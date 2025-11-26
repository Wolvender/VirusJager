using UnityEngine;

public class SpeedrunTimer : MonoBehaviour
{
    public static SpeedrunTimer instance;

    public float timeElapsed = 0f;
    public bool isRunning = false;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (isRunning)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        timeElapsed = 0f;
        Debug.Log("Timerstarted");
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"🏁 Final Time: {timeElapsed:F2} seconds");
    }

}
