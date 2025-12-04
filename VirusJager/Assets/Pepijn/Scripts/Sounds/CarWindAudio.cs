using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarWindAudio : MonoBehaviour
{
    [Header("References")]
    public G29Controller car;
    private AudioSource windAudio;

    [Header("Wind Settings")]
    public float minWindSpeed = 4f;         // minimal speed before wind starts
    public float maxWindVolume = 1f;        // volume at max speed
    public float minPitch = 0.8f;           // pitch when barely hearing wind
    public float maxPitch = 1.6f;           // pitch at top speed
    public float fadeSmooth = 3f;           // smoothing

    private float targetVol;
    private float targetPitch;

    void Start()
    {
        windAudio = GetComponent<AudioSource>();

        windAudio.loop = true;
        windAudio.playOnAwake = false;
        windAudio.volume = 0f;
        windAudio.pitch = minPitch;
    }

    void Update()
    {
        if (car == null)
            return;

        float speed = car.CurrentSpeed;

        // ---- SPEED percent ----
        float percent = Mathf.InverseLerp(minWindSpeed, car.maxSpeed, speed);

        targetVol = Mathf.Lerp(0f, maxWindVolume, percent);
        targetPitch = Mathf.Lerp(minPitch, maxPitch, percent);

        // ---- SMOOTH ----
        windAudio.volume = Mathf.Lerp(windAudio.volume, targetVol, Time.deltaTime * fadeSmooth);
        windAudio.pitch = Mathf.Lerp(windAudio.pitch, targetPitch, Time.deltaTime * fadeSmooth);

        // ---- PLAY IF MOVING ----
        if (!windAudio.isPlaying && speed > 0.5f)
            windAudio.Play();

        // ---- STOP WHEN FULLY IDLE ----
        if (windAudio.isPlaying && speed <= 0.3f && windAudio.volume < 0.02f)
            windAudio.Stop();
    }
}
