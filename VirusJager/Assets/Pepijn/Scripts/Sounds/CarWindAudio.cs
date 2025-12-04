using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarWindAudio : MonoBehaviour
{
    [Header("References")]
    public G29Controller car;
    private AudioSource windAudio;

    [Header("Wind Settings")]
    public float minWindSpeed = 2f;       // wind starts here
    public float maxWindSpeed = 20f;      // wind fully maxed here

    [Range(0f, 0.4f)]
    public float maxVolume = 0.25f;       // very subtle max volume

    public float minPitch = 0.85f;        // gentle start
    public float maxPitch = 1.25f;        // subtle top

    public float smooth = 2.5f;           // smooth transitions

    private void Start()
    {
        windAudio = GetComponent<AudioSource>();
        windAudio.loop = true;
        windAudio.playOnAwake = false;
        windAudio.volume = 0f;
        windAudio.pitch = minPitch;
    }

    private void Update()
    {
        if (car == null) return;

        float speed = car.CurrentSpeed;

        // normalized percent for lerp
        float percent = Mathf.InverseLerp(minWindSpeed, maxWindSpeed, speed);

        // reduce volume a lot at low speeds
        float targetVol = Mathf.Lerp(0f, maxVolume, percent * percent);

        // pitch remains smooth
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, percent);

        // smooth fade
        windAudio.volume = Mathf.Lerp(windAudio.volume, targetVol, Time.deltaTime * smooth);
        windAudio.pitch = Mathf.Lerp(windAudio.pitch, targetPitch, Time.deltaTime * smooth);

        // always play while moving even slowly
        if (!windAudio.isPlaying && speed > 0.25f)
            windAudio.Play();

        // optional: stop when completely idle
        if (windAudio.isPlaying && speed < 0.1f && windAudio.volume < 0.01f)
            windAudio.Stop();
    }
}
