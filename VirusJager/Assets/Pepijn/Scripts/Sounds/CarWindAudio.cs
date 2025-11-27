using UnityEngine;

public class CarWindAudio : MonoBehaviour
{
    [Header("References")]
    public G29Controller car;       // sleep je car object hier
    public AudioSource windAudio;   // je looping wind audio source

    [Header("Wind Settings")]
    public float startWindSpeed = 10f; // start wind pas vanaf deze snelheid
    public float maxWindVolume = 1f;
    public float maxWindPitch = 1.3f;
    public float windSmooth = 2f;

    void Start()
    {
        if (windAudio != null)
        {
            windAudio.loop = true;
            windAudio.playOnAwake = false;
        }
    }

    void Update()
    {
        if (car == null || windAudio == null)
            return;

        float speed = car.CurrentSpeed;

        // ---- bepaal speedPercent alleen als we boven startWindSpeed zitten ----
        float speedPercent = 0f;
        if (speed > startWindSpeed)
        {
            speedPercent = (speed - startWindSpeed) / (car.maxSpeed - startWindSpeed);
            speedPercent = Mathf.Clamp01(speedPercent);
        }

        // ---- target volume/pitch ----
        float targetVol = Mathf.Lerp(0f, maxWindVolume, speedPercent);
        float targetPitch = Mathf.Lerp(1f, maxWindPitch, speedPercent);

        // ---- smooth transitions ----
        windAudio.volume = Mathf.Lerp(windAudio.volume, targetVol, Time.deltaTime * windSmooth);
        windAudio.pitch = Mathf.Lerp(windAudio.pitch, targetPitch, Time.deltaTime * windSmooth);

        // ---- play audio als hij nog niet speelt ----
        if (!windAudio.isPlaying && speed > startWindSpeed * 0.1f) // kleine marge zodat hij niet meteen begint
            windAudio.Play();
    }
}
